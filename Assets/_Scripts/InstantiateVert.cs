using UnityEngine;
using System.Collections;

public class InstantiateVert : MonoBehaviour
{

    /// <summary>

    /// Compute a skinned mesh's deformation.

    /// 

    /// The script must be attached aside a SkinnedMeshRenderer,

    /// which is only used to get the bone list and the mesh

    /// (it doesn't even need to be enabled).

    /// 

    /// Make sure the scripts accessing the results run after this one

    /// (otherwise you'll have a 1-frame delay),

    /// </summary>

   // @HideInInspector
    Mesh mesh;
    // @HideInInspector
    public SkinnedMeshRenderer skin;
 
 
    private int vertexCount = 0;
    
    private Vector3[] vertices;
    
    Vector3[] normals;
 
 
    public GameObject ObjectToInstantiate;


    void Start()
    {

        skin = GetComponent<SkinnedMeshRenderer>();

        mesh = skin.sharedMesh;
        print(skin.name);


        vertexCount = mesh.vertexCount;

        //Vector3[] vertices = mesh.vertices;

        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    print(vertices[i]);
        //}

        vertices = new Vector3[vertexCount];//the vertices that have skin weights that need to be updated every frame (check line 115)

        normals = new Vector3[vertexCount];

        //animation example
        for (int b = 0; b < mesh.vertexCount; b++)
        {
            //GameObject cube= new GameObject.CreatePrimitive(PrimitiveType.Cube);//the gameobject that is being instantiated
            GameObject cube = Instantiate(ObjectToInstantiate);
            cube.name = b.ToString();
            //cube.AddComponent.<Rigidbody>();

            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        }
        print(skin.bones.Length);
    }



    void Update()
    {
        //print(skin.bones.Length);
        Matrix4x4[] boneMatrices = new Matrix4x4[skin.bones.Length];// this is an array of 4x4 matrices; to allow for transformations in 3D space for each of the vertices; there will be 75 bones and their resp. bone matrices

        //print(boneMatrices.Length);
        for (int i = 0; i < boneMatrices.Length; i++)
        {

            boneMatrices[i] = skin.bones[i].localToWorldMatrix * mesh.bindposes[i];//read the transform from local to world space and multiply with the bind pose of each of the bones in the hierarchy

        }


        for (int b = 0; b < mesh.vertexCount; b++)
        {

            BoneWeight weight = mesh.boneWeights[b];//bone weights of each vertex in the Mesh

            //print(b);
            //Each vertex is skinned with up to four bones. All weights should sum up to one. Weights and bone indices should be defined in the order of decreasing weight. If a vertex is affected by less than four bones, the remaining weights should be zeroes 
            Matrix4x4 bm0 = boneMatrices[weight.boneIndex0];// index of first bone

            Matrix4x4 bm1 = boneMatrices[weight.boneIndex1];// index of second bone

            Matrix4x4 bm2 = boneMatrices[weight.boneIndex2];// index of third bone

            Matrix4x4 bm3 = boneMatrices[weight.boneIndex3];// index of fourth bone



            Matrix4x4 vertexMatrix = new Matrix4x4();



            for (int n = 0; n < 16; n++)
            {//each vertex in the vertexmatrix (16 elements of a 4x4 matrix) is a summation of all possible (up to 4) skinning vertex weights influencing a given bone

                vertexMatrix[n] =

                    bm0[n] * weight.weight0 +

                    bm1[n] * weight.weight1 +

                    bm2[n] * weight.weight2 +

                    bm3[n] * weight.weight3;

            }



            vertices[b] = vertexMatrix.MultiplyPoint3x4(mesh.vertices[b]);
            normals[b] = vertexMatrix.MultiplyVector(mesh.normals[b]);

            //animation example
            GameObject fetch = GameObject.Find(b.ToString());
            //Debug.Log(fetch.transform.position);
            fetch.transform.position = vertices[b];
            //Debug.Log(fetch.transform.position);
        }


    }
  
}