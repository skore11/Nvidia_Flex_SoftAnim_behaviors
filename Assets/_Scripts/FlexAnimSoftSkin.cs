﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
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


    [RequireComponent(typeof(FlexSoftActor))]
    public class FlexAnimSoftSkin : MonoBehaviour
    {
        public SkinnedMeshRenderer referenceAnimSkin; // hmmmmmm this cannot be the local skinnedmeshrenderer,
        // as that one is used by FlexSoftSkinning and its bones are set by that!
        // SHOULD be using the original animation all along anyway! also in the other setup probably!
        // i.e. the default avatar playing the animation (potentially offscreen)

        private FlexSoftActor m_actor;
        // caches for values copied in and out of the flex subsystem:
        private Vector4[] m_particles;
        private Vector3[] m_velocities;

        // TODO: clearly check/explain what function these member variables fulfill:

        // unique index of mesh vertices to map on to Flex particle positions
        private List<int> unique_Index = new List<int>();
        private List<Vector3> VertOffsetVectors = new List<Vector3>();

        private List<Vector3> particlePositions = new List<Vector3>(); // world Flex particle positions

        // Needed to update the mass particle positions after assigning bone weights from vertex mapping
        private List<Vector3> particleRestPositions = new List<Vector3>();

        private WeightList[] particleNodeWeights; // one per node (vert). Weights of standard mesh

        private Vector3[] shapeAnimVectors; // used when updating particle positions

        private bool firstRun = true;

        private void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
            if (referenceAnimSkin == null)
            {
                Debug.LogError("FlexAnimSoftSkin cannot work without a reference animation (Note: NOT the Skin of the Flex Object itself!)");
            }
        }

        private void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
            shapeAnimVectors = new Vector3[m_particles.Length];
            string debugOutput = "Animating " + m_actor.name + " based on Skin " + referenceAnimSkin.name;
            debugOutput += "\n Indices: " + m_actor.indexCount + " from " + m_actor.indices[0] + " to " + m_actor.indices[m_actor.indexCount - 1];
            debugOutput += "\n mass Scale: " + m_actor.massScale;
            debugOutput += "\n particle Group: " + m_actor.particleGroup;
            debugOutput += "\n reference Shape: " + m_actor.asset.referenceShape;
            debugOutput += "\n shapeindices length: " + m_actor.asset.shapeCenters.Length;
            debugOutput += "\n fixed particles: " + m_actor.asset.fixedParticles.Length;
            Debug.Log(debugOutput);
        }

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            if (firstRun)
            {
                firstRun = false; // only run this once:
                Debug.Log("Creating Vertex Mapping");
                // TODO: this probably needs to be done outside of firstRun, as particle positions are
                // used later too (?)
                _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
                // TODO: re-check full algorithm below...
                Mesh mesh = referenceAnimSkin.sharedMesh;
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
            }
            UpdateParticlePositions(_particleData);
        }

        public void UpdateParticlePositions(FlexContainer.ParticleData _particleData)
        {
            // set all particle postions to zero Vector first
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
                    Transform t = referenceAnimSkin.bones[wList.boneIndex];
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
            //for (int i = 0; i < particlePositions.Count; i++)
            //{
            int ppIndex = 0;
            Vector3 particlePos = new Vector3();
            Vector3 _tempVector3;
            //int bigCount = 0;
            for (int i = 0; i < m_particles.Length; i++)
            {

                int primIndex = i;
                particlePos.x = m_particles[i].x;
                particlePos.y = m_particles[i].y;
                particlePos.z = m_particles[i].z;
                _tempVector3 = particlePositions[ppIndex] - particlePos;

                shapeAnimVectors[primIndex].x = _tempVector3.x;// * Time.fixedDeltaTime;
                shapeAnimVectors[primIndex].y = _tempVector3.y;// * Time.fixedDeltaTime;
                shapeAnimVectors[primIndex].z = _tempVector3.z;// * Time.fixedDeltaTime;

                ppIndex++;

            }
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);

            //print(shapeAnimVectors.Length);
            for (int i = 0; i < shapeAnimVectors.Length; i++)
            {
                // looks like the main slowdown is that this function here eventually
                // triggers FlexActor.ApplyImpulses and that then uses
                // FlexContainer.SetVelocity internally, so setting one velocity at a time,
                // plus some code to avoid applying tiny impulses or impulses to tiny masses.
                // But we are effectively applying impulses to everything all the time.
                // So we should probably use
                //_particleData.GetVelocities
                // and
                //_particleData.SetVelocities
                // directly
                //m_actor.ApplyImpulse(shapeAnimVectors[i]*5000, i);

                // this now replicates roughly what ApplyImpulses in Actor would do:
                // TODO: why 5000? what are the units here?
                // it should be impulse divide by particle mass (which is 1/w):
                _tempVector3 = shapeAnimVectors[i] * m_particles[i].w;
                m_velocities[i] += _tempVector3;
            }
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
        }

        private void SetBoneWeights(List<Vector3> uniqueParticlePositions, List<int> uniqueParticleIndices)
        {
            // Cache used values rather than accessing straight from the mesh on the loop below
            Vector3[] cachedVertices = referenceAnimSkin.sharedMesh.vertices;

            Matrix4x4[] cachedBindposes = referenceAnimSkin.sharedMesh.bindposes;
            BoneWeight[] cachedBoneWeights = referenceAnimSkin.sharedMesh.boneWeights;

            // Make a CWeightList for each bone in the skinned mesh
            WeightList[] nodeWeights = new WeightList[referenceAnimSkin.bones.Length];
            for (int i = 0; i < referenceAnimSkin.bones.Length; i++)
            {
                nodeWeights[i] = new WeightList();
                nodeWeights[i].boneIndex = i;
                nodeWeights[i].transform = referenceAnimSkin.bones[i];
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
    }
}
