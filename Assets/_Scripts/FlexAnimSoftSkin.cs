using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace NVIDIA.Flex
{
    [System.Serializable]
    public class VertexWeight
    {
        public int index;
        public Vector3 localPosition;
        public float weight;

        public VertexWeight(int i, Vector3 p, float w)
        {
            index = i;
            localPosition = p;
            weight = w;
        }
    }

    [System.Serializable]
    public class WeightList
    {
        private Transform _temp; // cached on use, not serialized
        public Transform transform
        {
            get
            {
                if (_temp == null)
                {
                    _temp = new GameObject().transform;
                    _temp.position = pos;
                    _temp.rotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
                    _temp.localScale = scale;
                }
                return _temp;
            }
            set
            {
                pos = value.position;
                rot = new Vector4(value.rotation.x, value.rotation.y, value.rotation.z, value.rotation.w);
                scale = value.localScale;
            }
        }
        public int boneIndex; // for transform
        public Vector3 pos;
        public Vector4 rot;
        public Vector3 scale;

        public List<VertexWeight> weights = new List<VertexWeight>();
    }



    


    //[ExecuteInEditMode]
    public class FlexAnimSoftSkin : MonoBehaviour
    {

        float refreshRate = 1.0f / 30.0f;
        float timeDelta;

        FlexSoftActor m_actor;
        BoxCollider locked_box;
        //FlexContainer m_container;
        private Vector4[] m_particles;
        //private int indices;
        int start = 0;
        public int frameInterval = 30;
        private int framesToNextPrint = 0;

        public SkinnedMeshRenderer Skin;

        private List<int> unique_Index = new List<int>();//unique index of mesh vertices to map on to Flex particle positions
        private List<Vector3> VertOffsetVectors = new List<Vector3>();

        private List<Vector3> particlePositions = new List<Vector3>(); // world Flex particle positions

        //Needed to update the mass particle positions after assigning bone weights from vertex mapping
        private List<Vector3> particleRestPositions = new List<Vector3>();

        private WeightList[] particleNodeWeights; // one per node (vert). Weights of standard mesh

        Vector3[] _cachedVertices;
        Matrix4x4[] _cachedBindposes;
        BoneWeight[] _cachedBoneWeights;

        bool map;

        private bool firstRun = true;

        private void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
            if (Skin == null)
            {
                Skin = GetComponent<SkinnedMeshRenderer>();
            }

        }

    private int GetNearestVertIndex(Vector3 particlePos, Vector3[] cachedVertices)
    {
        float nearestDist = float.MaxValue;
        int nearestIndex = -1;
        for (int i = 0; i < cachedVertices.Length; i++)
        {
            float dist = Vector3.Distance(particlePos, cachedVertices[i]);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIndex = i;
            }
        }
        return nearestIndex;
    }

    public Vector3 GetNearestVertPos(Vector3 particlePos, Vector3[] cachedVertices)
        //public int GetNearestVertIndex(Vector3 particlePos, Vector3[] cachedVertices)
        {
            float nearestDist = float.MaxValue;
            int nearestIndex = -1;
            Vector3 nearestVertexPosition = new Vector3(0.0f, 0.0f, 0.0f);
            for (int i = 0; i < cachedVertices.Length; i++)
            {
                float dist = Vector3.Distance(particlePos, cachedVertices[i]);
                //Debug.Log("Mass pos: " + particlePos + "mesh vertex pos: " + cachedVertices[i]);
                //Debug.DrawLine(particlePos, cachedVertices[i], Color.blue, 5.5f);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestVertexPosition = cachedVertices[i];
                    nearestIndex = i;
                }
            }
            //return nearestIndex;
            return nearestVertexPosition;
        }

    private void SetBoneWeights( List<Vector3> uniqueParticlePositions, List<int> uniqueParticleIndices)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = Skin;

            // Cache used values rather than accessing straight from the mesh on the loop below
            Vector3[] cachedVertices = skinnedMeshRenderer.sharedMesh.vertices;

            Matrix4x4[] cachedBindposes = skinnedMeshRenderer.sharedMesh.bindposes;
            BoneWeight[] cachedBoneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;

            // Make a CWeightList for each bone in the skinned mesh
            WeightList[] nodeWeights = new WeightList[skinnedMeshRenderer.bones.Length];
            for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            {
                nodeWeights[i] = new WeightList();
                nodeWeights[i].boneIndex = i;
                nodeWeights[i].transform = skinnedMeshRenderer.bones[i];
            }

            for (int uniqueIndex = 0; uniqueIndex < uniqueParticleIndices.Count; uniqueIndex++)
            {
                //Vector3 particlePos = uniqueParticlePositions[uniqueIndex];
                //int i = GetNearestSkinVertIndex(particlePos, ref cachedVertices);
                int i = uniqueParticleIndices[uniqueIndex];
                Vector3 particlePos = uniqueParticlePositions[i];
                //Debug.Log(i);
                //nearestVertIndex.Add(i);
                //uniqueIndex.Add(uniqueIndex);
                BoneWeight bw = cachedBoneWeights[i];

                if (bw.weight0 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex0].MultiplyPoint3x4(particlePos);// cachedVertices[i]);
                    nodeWeights[bw.boneIndex0].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight0));
                }
                if (bw.weight1 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex1].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
                    nodeWeights[bw.boneIndex1].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight1));
                }
                if (bw.weight2 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex2].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
                    nodeWeights[bw.boneIndex2].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight2));
                }
                if (bw.weight3 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex3].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
                    nodeWeights[bw.boneIndex3].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight3));
                }
            }
            particleNodeWeights = nodeWeights;
        }

        private void Start()
        {
            //Mesh mesh = Skin.sharedMesh;
            //Vector3[] cachedVertices = mesh.vertices;
            //List<Vector3> tempCache = new List<Vector3>();
            //for (int i = 0; i < cachedVertices.Length; i++)
            //{
            //    tempCache.Add(cachedVertices[i]);
            //}
            m_actor.onFlexUpdate += OnFlexUpdate;
            m_particles = new Vector4[m_actor.indexCount];
            Debug.Log("Created array of size: " + m_actor.indexCount);
            Debug.Log("All indices: " + m_actor.indices[0] + " to " + m_actor.indices[m_actor.indexCount - 1]);
            Debug.Log(" mass Scale: " + m_actor.massScale);
            Debug.Log(" particle Group: " + m_actor.particleGroup);
            Debug.Log(" reference Shape: " + m_actor.asset.referenceShape);
            Debug.Log(" shapeindices length: " + m_actor.asset.shapeCenters.Length);
            Debug.Log(" fixed particles: " + m_actor.asset.fixedParticles.Length);

            map = true;

        }

        IEnumerator WaitForParticles()
        {
            while (m_particles[0].x == 0.0f)
            {
                yield return null;
            }
        }
        #region

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            //if (this.framesToNextPrint > 0)
            //{
            //    this.framesToNextPrint -= 1;
            //    return;
            //}
            //this.framesToNextPrint = this.frameInterval;

            if (map)
            {
                _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
                Mesh mesh = Skin.sharedMesh;
                Vector3[] cachedVertices = mesh.vertices;
                List<Vector3> tempCache = new List<Vector3>();
                for (int i = 0; i < cachedVertices.Length; i++)
                {
                    tempCache.Add(cachedVertices[i]);
                }

                foreach (var i in m_particles)
                {
                    Vector3 restPos = new Vector3();
                    restPos.x = i.x;
                    restPos.y = i.y;
                    restPos.z = i.z;

                    particleRestPositions.Add(restPos);

                    int nearestIndexforParticle = GetNearestVertIndex(restPos, cachedVertices);

                    Vector3 nearestVertexPosforParticle = GetNearestVertPos(restPos, cachedVertices);
                    Vector3 VertOffset = restPos - nearestVertexPosforParticle;

                    VertOffsetVectors.Add(VertOffset);
                    unique_Index.Add(nearestIndexforParticle);
                }
                SetBoneWeights( tempCache, unique_Index);


                particlePositions = particleRestPositions;
                tempCache = null;
                map = false;
            }

            //_particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            //Vector4 testVector = _particleData.GetParticle(m_actor.indices[0]);

            if (firstRun)
            {
                firstRun = false;

                _cachedVertices = Skin.sharedMesh.vertices;
                _cachedBindposes = Skin.sharedMesh.bindposes;
                _cachedBoneWeights = Skin.sharedMesh.boneWeights;

                List<Vector3> tempCache = new List<Vector3>();
                for (int i = 0; i < _cachedVertices.Length; i++)
                {
                    tempCache.Add(_cachedVertices[i]);
                }
                SetBoneWeights(tempCache, unique_Index);
                particlePositions = particleRestPositions;


                UpdateParticlePositions();
            }
            else
            {
                if (timeDelta < refreshRate)
                    return;
                UpdateParticlePositions();
            }

            //Debug.Log(" OnFlexUpdate in our claSS!!!!!! got particle: " + testVector);
            //Debug.Log(" fixed particles: " + m_actor.asset.fixedParticles.Length);
            //Debug.Log(" Indices: " + m_actor.indices[0] + "  " + m_actor.indices[20]);
            //Debug.Log(" OnFlexUpdate in our claSS!!!!!! got particle: " + m_particles[0] + m_particles[1]
            //    + m_particles[m_actor.indexCount / 2] + m_particles[m_actor.indexCount / 4]);

        }
        #endregion

        public void UpdateParticlePositions()
        {
            //set all particle postions to zero Vector first
            for (int i = 0; i < particlePositions.Count; i++)
            {
                //particlePositions[i] = Spawner.nextPositions[i];
                particlePositions[i] = Vector3.zero;
            }


            // Now get the local positions of all weighted indices...
            foreach (WeightList wList in particleNodeWeights)
            {
                //print(wList);
                foreach (VertexWeight vw in wList.weights)
                {
                    Transform t = Skin.bones[wList.boneIndex];
                    particlePositions[vw.index] += t.localToWorldMatrix.MultiplyPoint3x4(vw.localPosition) * vw.weight + VertOffsetVectors[vw.index];

                    //print(particlePositions[vw.index]);
                }
            }

            //print(particlePositions.Count);

            if (particlePositions.Count == 0)
            {
                return;
            }
            //print(particlePositions.Count);
            // Now convert each point into local coordinates of this object.
            //List<Vector3> nextPos = new List<Vector3>(particlePositions.Count);
            Vector3[] shapeAnimVectors = new Vector3[m_particles.Length];

            //for (int i = 0; i < particlePositions.Count; i++)
            //{
            int ppIndex = 0;
            //int bigCount = 0;
            for (int i = 0; i < m_particles.Length; i++)
            {

                int primIndex = i;
                Vector3 particlePos = new Vector3();
                particlePos.x = m_particles[i].x;
                particlePos.y = m_particles[i].y;
                particlePos.z = m_particles[i].z;
                Vector3 temp = particlePositions[ppIndex] - particlePos;

                shapeAnimVectors[primIndex].x = temp.x;// * Time.fixedDeltaTime;
                shapeAnimVectors[primIndex].y = temp.y;// * Time.fixedDeltaTime;
                shapeAnimVectors[primIndex].z = temp.z;// * Time.fixedDeltaTime;

                ppIndex++;

            }
            for (int i = 0; i < shapeAnimVectors.Length; i++)
            {
                m_actor.ApplyImpulse(shapeAnimVectors[i]*5000, i);
            }
        }

    }
}
