using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace NVIDIA.Flex
{

    //public class VertMapBuilder
    //{
    //    public SkinnedMeshRenderer skin;
    //    public VertMapAsset vertMapAsset;

    //    private int GetNearestSkinVertIndex(Vector3 particlePos, ref Vector3[] cachedVertices)
    //    {
    //        float nearestDist = float.MaxValue;
    //        int nearestIndex = -1;
    //        for (int i = 0; i < cachedVertices.Length; i++)
    //        {
    //            float dist = Vector3.Distance(particlePos, cachedVertices[i]);
    //            if (dist < nearestDist)
    //            {
    //                nearestDist = dist;
    //                nearestIndex = i;
    //            }
    //        }
    //        return nearestIndex;
    //    }

    //    private void SetBoneWeights(ref List<Vector3> uniqueParticlePositions, ref List<int> uniqueParticleIndices)
    //    {
    //        SkinnedMeshRenderer skinnedMeshRenderer = skin;

    //        // Cache used values rather than accessing straight from the mesh on the loop below
    //        Vector3[] cachedVertices = skinnedMeshRenderer.sharedMesh.vertices;

    //        Matrix4x4[] cachedBindposes = skinnedMeshRenderer.sharedMesh.bindposes;
    //        BoneWeight[] cachedBoneWeights = skinnedMeshRenderer.sharedMesh.boneWeights;

    //        // Make a CWeightList for each bone in the skinned mesh
    //        WeightList[] nodeWeights = new WeightList[skinnedMeshRenderer.bones.Length];
    //        for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
    //        {
    //            nodeWeights[i] = new WeightList();
    //            nodeWeights[i].boneIndex = i;
    //            nodeWeights[i].transform = skinnedMeshRenderer.bones[i];
    //        }

    //        for (int uniqueIndex = 0; uniqueIndex < uniqueParticleIndices.Count; uniqueIndex++)
    //        {
    //            Vector3 particlePos = uniqueParticlePositions[uniqueIndex];
    //            int i = GetNearestSkinVertIndex(particlePos, ref cachedVertices);
    //            //Debug.Log(i);
    //            vertMapAsset.nearestVertIndex.Add(i);
    //            vertMapAsset.uniqueIndex.Add(uniqueIndex);
    //            BoneWeight bw = cachedBoneWeights[i];

    //            if (bw.weight0 != 0.0f)
    //            {
    //                Vector3 localPt = cachedBindposes[bw.boneIndex0].MultiplyPoint3x4(particlePos);// cachedVertices[i]);
    //                nodeWeights[bw.boneIndex0].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight0));
    //            }
    //            if (bw.weight1 != 0.0f)
    //            {
    //                Vector3 localPt = cachedBindposes[bw.boneIndex1].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
    //                nodeWeights[bw.boneIndex1].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight1));
    //            }
    //            if (bw.weight2 != 0.0f)
    //            {
    //                Vector3 localPt = cachedBindposes[bw.boneIndex2].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
    //                nodeWeights[bw.boneIndex2].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight2));
    //            }
    //            if (bw.weight3 != 0.0f)
    //            {
    //                Vector3 localPt = cachedBindposes[bw.boneIndex3].MultiplyPoint3x4(particlePos);//cachedVertices[i]);
    //                nodeWeights[bw.boneIndex3].weights.Add(new VertexWeight(uniqueIndex, localPt, bw.weight3));
    //            }
    //        }
    //    }
    //}


    [ExecuteInEditMode]
    public class FlexAnimSoftSkin : MonoBehaviour
    {
        FlexSoftActor m_actor;
        //FlexContainer m_container;
        private Vector4[] m_particles;
        private int indices;
        int start = 0;
        //private void Start()
        //{
        //    m_actor = GetComponent<FlexSoftActor>();
        //}

        #region

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            //_particleData.GetParticles(start, m_actor.indexCount, m_particles);
            //FlexExt.GetActiveList(, ref indices);
            string test = Marshal.PtrToStringAnsi(_particleData.particleData.particles);
            //FlexExt.Instance instance= m_actor.handle.instance;
            //instance.
            //base.OnFlexUpdate(_particleData);
            Debug.Log(test);
        }
        #endregion
    }
}
