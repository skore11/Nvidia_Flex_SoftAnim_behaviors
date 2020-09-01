using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

#if UNITY_EDITOR
using UnityEditor; // required to build assets
#endif

public class myVertMapAssetBuilder
{
    public FlexShapeMatching flexShapeMatching;
    public VertMapAsset vertMapAsset;
    public SkinnedMeshRenderer skin;

    //constructor
    public myVertMapAssetBuilder(FlexShapeMatching flexShapeMatching, VertMapAsset vertMapAsset, SkinnedMeshRenderer skin)
    {
        this.flexShapeMatching = flexShapeMatching;
        this.vertMapAsset = vertMapAsset;
        this.skin = skin;
    }

    //return a list of vectors postions of particles within each shape; rest postions of particles withn each shape are provided from flex shape matching
    private List<Vector3> GetFlexShapeMatchingPositions()
    {
        List<Vector3> vertPos = new List<Vector3>();
        FlexShapeMatching shapes = this.flexShapeMatching;
        int shapeIndex = 0;
        int shapeIndexOffset = shapes.m_shapesIndex;
        int shapeStart = 0;

        for (int s = 0; s < shapes.m_shapesCount; s++)
        {
            shapeIndex++;
            int shapeEnd = shapes.m_shapeOffsets[s];
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                Vector3 pos = flexShapeMatching.m_shapeRestPositions[i] + shapes.m_shapeCenters[s];
                vertPos.Add(pos);
                shapeIndexOffset++;

            }

            shapeStart = shapeEnd;
        }
        return vertPos;
    }
    //Bone weights need to be transferred from skinned vertices to flex particles stored in weightList of vertmapasset
    //Pure euclidean distance calcualted to find nearest position of flex particle to skin cached vertices
    //just transfer bone weight and assign to nodeweight array in vertmapasset that can be stored and used in Update Particle positions
    //max 4 bones can affect each mapped skinned vertex and hence mapped flex particle 
    //concept is bind poses
    public void SetBoneWeights(ref List<Vector3> uniqueParticlePositions, ref List<int> uniqueParticleIndices)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = skin;

        Vector3[] cachedVertices = skinnedMeshRenderer.sharedMesh.vertices;

        Matrix4x4[] cachedBindPoses = skinnedMeshRenderer.sharedMesh.bindposes;
        BoneWeight[] cachedBoneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;

        WeightList[] nodeWeights = new WeightList[skinnedMeshRenderer.bones.Length];
        for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
        {
            nodeWeights[i] = new WeightList();
            nodeWeights[i].boneIndex = i;
            nodeWeights[i].transform = skinnedMeshRenderer.bones[i];
        }


    }

    //While creating asset check if the vertex from all shape matching positions is present in our unique flex particle positions 
    //if it does not exist then add it to the unique particles array that is used for particle shape matching
    //bone weights are set during creation of asset(scriptable object)
}

public class TestFlexAnimation : FlexProcessor
{
    public bool rebuildVertmapAsset = false;
    public VertMapAsset vertMapAsset;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    private FlexShapeMatching flexShapeMatching;
    private bool firstRun = true;
    public Vector3[] particlePositions;
    public bool drawVertmapAsset = false;
    public GameObject shapeMatchingGO;
    public FlexParticles flexparticles;

    Vector3[] _cachedVertices;
    Matrix4x4[] _cachedBindPoses;
    BoneWeight[] _cachedBoneWeights;
    float refreshRate = 1.0f / 30.0f;
    float timeDelta;

     void OnEnable()
    {
        if (flexShapeMatching == null)
        {
            flexShapeMatching = shapeMatchingGO.GetComponent<FlexShapeMatching>();
        }
    }

    public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (enabled == false)
            return;

        if (firstRun)
        {
            firstRun = false;

            _cachedVertices = skinnedMeshRenderer.sharedMesh.vertices;
            _cachedBindPoses = skinnedMeshRenderer.sharedMesh.bindposes;
            _cachedBoneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;

            bool createAsset = vertMapAsset == null || rebuildVertmapAsset;

            if (vertMapAsset == null)
            {
#if UNITY_EDITOR
                vertMapAsset = ScriptableObject.CreateInstance<VertMapAsset>();
                AssetDatabase.CreateAsset(vertMapAsset, "Assets/" + this.name + "VertMapAsset.asset");
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = vertMapAsset;
#endif
            }

            if (createAsset)
            {
                vertMapAsset.vertexParticleMap = new List<int>();
                VertMapAssetBuilder vertMapAssetBuilder = new VertMapAssetBuilder(flexShapeMatching, vertMapAsset, skinnedMeshRenderer);
                vertMapAssetBuilder.CreateAsset();
            }

            particlePositions = new Vector3[vertMapAsset.particleRestPositions.Count];
            //UpdateParticlePostions();
        }

        else
        {
            if (timeDelta < refreshRate)
                return;

            //UpdateParticlePositions();
            //MatchShapes();

            while (timeDelta >= refreshRate)
                timeDelta -= refreshRate;
            }
    }

    //mapping the skinned cached vertices to flex particles
    //because of the trasnfer of bone weights and local to world transforms for bone indices, the flex particles move with same motion
    //vertex weights are converted to flex positions from skin vertices by using get nearest neighbor fucntion while mapping (See vertex paritcle map in vertmapasset)
    public void UpdateParticlePositions()
    {
        float startTime = Time.time;

        for (int i = 0; i < particlePositions.Length; i++)
        {
            particlePositions[i] = Vector3.zero;
        }

        foreach (WeightList wList in vertMapAsset.particleNodeWeights)
        {
            foreach (VertexWeight vw in wList.weights)
            {
                Transform t = skinnedMeshRenderer.bones[wList.boneIndex];
                particlePositions[vw.index] += t.localToWorldMatrix.MultiplyPoint3x4(vw.localPosition) * vw.weight;
            }
        }

        for (int i = 0; i < particlePositions.Length; i++)
        {
            particlePositions[i] = transform.InverseTransformPoint(particlePositions[i]);
        }
    }


    //For every shape and its constituent particles match pos to pos of mapped index made in vermaptasset;  this is called in postcontainerupdate() 
    //the mapped index list contains the bone weight data for moving array of flex particles 
    
    private void MatchShapes()
    {
        FlexShapeMatching shapes = this.flexShapeMatching;

        int shapeIndex = 0;
        int shapeIndexOffset = 0;
        int shapeStart = 0;
        int vertIndex = 0;


        for (int s = 0; s < shapes.m_shapesCount; s++)
        {
            Vector3 shapeCenter = new Vector3();
            shapeIndex++;

            int shapeEnd = shapes.m_shapeOffsets[s];
            int shapeCount = shapeEnd - shapeStart;

            int origShapeIndexOffset = shapeIndexOffset;
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                int mappedIndex = vertMapAsset.vertexParticleMap[vertIndex];
                Vector3 pos = particlePositions[mappedIndex];
                shapes.m_shapeRestPositions[shapeIndexOffset] = pos;
                shapeCenter += pos;
                shapeIndexOffset++;
                vertIndex++;
            }

            shapeCenter /= shapeCount;

            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                Vector3 pos = shapes.m_shapeRestPositions[origShapeIndexOffset];
                pos -= shapeCenter;
                shapes.m_shapeRestPositions[origShapeIndexOffset] = pos;
                origShapeIndexOffset++;
            }

            shapeStart = shapeEnd;
        }
        
    }

}
