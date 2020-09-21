using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    [System.Serializable]
    public class ParticleBoneWeight
    {
        public int particleIndex;
        public Vector3 localPosition;
        public float weight;

        public ParticleBoneWeight(int i, Vector3 p, float w)
        {
            particleIndex = i;
            localPosition = p;
            weight = w;
        }
    }

    [System.Serializable]
    public class WeightList
    {
        public int boneIndex; // for transform
        public List<ParticleBoneWeight> weights = new List<ParticleBoneWeight>();
    }

    /**
     * Animate a Flex object by trying to mimic an animation as it plays.
     * Adjust the velocities of particles to nudge them into the positions they should have
     * based on the current state of the mesh in the animation. But rather than re-rasterize
     * the mesh every frame, create a mapping of particles to vertices 
     */
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

        // Performance note: all of the following could be arrays instead of Lists for a small performance boost

        // index of the mesh vertices that are closest to each Flex particle in the rest state
        // the bone weights of that vertex will be used to animate that particle:
        private List<int> nearestVertexIndexForParticle = new List<int>();
        // The offset fro the nearest Vertex to "its" particle
        // (Note: we might not need that, if we change ParticleBoneWeight to just store the offset to the
        // particle rather than to the vertex?)
        private List<Vector3> VertOffsetVectors = new List<Vector3>();
        // local copy of the world position for all Flex particles
        private List<Vector3> particlePositions = new List<Vector3>();
        // Needed to update the mass particle positions after assigning bone weights from vertex mapping
        private List<Vector3> particleRestPositions = new List<Vector3>();
        // for each bone of the original mesh, store a list of weights for the particles it should influence:
        private WeightList[] particleBoneWeights;

        private Vector3[] particleDisplacementVector; // used when updating particle positions

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
            // local caches used during the update of particle positions based on the reference animation:
            m_particles = new Vector4[m_actor.indexCount];
            m_velocities = new Vector3[m_particles.Length];
            // TODO: rename this one:
            particleDisplacementVector = new Vector3[m_particles.Length];
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
            // Fill a local copy of the particles:
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            if (firstRun)
            {
                firstRun = false; // only run this once:
                Debug.Log("Creating Mapping from Particles to Bones based on closest Vertex of the original mesh!");                
                // TODO: re-check full algorithm below...
                Vector3[] cachedMeshVertices = referenceAnimSkin.sharedMesh.vertices;
                foreach (var i in m_particles)
                {
                    Vector3 restPos = new Vector3();
                    restPos.x = i.x;
                    restPos.y = i.y;
                    restPos.z = i.z;
                    particleRestPositions.Add(restPos);
                    particlePositions.Add(restPos);
                    int nearestIndexforParticle = GetNearestVertIndex(restPos, cachedMeshVertices);
                    Vector3 VertOffset = restPos - cachedMeshVertices[nearestIndexforParticle];
                    VertOffsetVectors.Add(VertOffset);
                    nearestVertexIndexForParticle.Add(nearestIndexforParticle);
                }
                SetBoneWeights(cachedMeshVertices, nearestVertexIndexForParticle);
            }
            else // no update on firstRun! (that assumes that the animation starts from the rest position)
            {
                UpdateParticlePositions(_particleData);
            }
        }

        public void UpdateParticlePositions(FlexContainer.ParticleData _particleData)
        {
            // "Zero out" all particle postions first
            for (int i = 0; i < particlePositions.Count; i++)
            {
                particlePositions[i] = Vector3.zero;
            }
            // For each bone, check all particles it should affect and add the effect to that particlePosition:
            foreach (WeightList wList in particleBoneWeights)
            {
                foreach (ParticleBoneWeight pbw in wList.weights)
                {
                    // use the current transform of the bone to add its contribution to the new particle position:
                    Transform t = referenceAnimSkin.bones[wList.boneIndex];
                    // Adding the VertOffsetVector moves it from the original vertex to the particle
                    particlePositions[pbw.particleIndex] += t.localToWorldMatrix.MultiplyPoint3x4(pbw.localPosition) * pbw.weight + VertOffsetVectors[pbw.particleIndex];
                }
            }
            // We now have the particlePositions as they should be based on the animation,
            // BUT at the position of the reference animation's root bone in world space.
            // First move it back to local space relative to its root bone.
            // And then we should have to move it back to world space relative to this GameObject,
            // but for some reason that is not needed - but it breaks when this GameObject is rotated
            // (didn't test scaling). There must be something in the Flex setup that moves the points? :
            for (int i = 0; i < particlePositions.Count; i++)
            {
                particlePositions[i] = referenceAnimSkin.rootBone.transform.InverseTransformPoint(particlePositions[i]);
                //particlePositions[i] = this.transform.TransformPoint(particlePositions[i]);
            }
            // Now we should have the target particlePositions
            if (particlePositions.Count == 0) // this check shouldn't be needed now without uFlex, but leaving it in
            {
                return;
            }
            // Now for each particle, calculate how far it is from its intended (animation-)position:
            Vector3 _tempVector3;
            for (int particleIndex = 0; particleIndex < m_particles.Length; particleIndex++)
            {
                // casting a Vector4 to a Vector3 automatically discards the w component:
                _tempVector3 = particlePositions[particleIndex] - (Vector3) m_particles[particleIndex];
                particleDisplacementVector[particleIndex].x = _tempVector3.x;
                particleDisplacementVector[particleIndex].y = _tempVector3.y;
                particleDisplacementVector[particleIndex].z = _tempVector3.z;
            }
            // Now apply an appropriate velocity change to nudge the particle into the right direction:
            // Get/Set all velocities at the same time, as this is the most performant method:
            _particleData.GetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
            for (int i = 0; i < particleDisplacementVector.Length; i++)
            {
                // Note: we replicate roughly what ApplyImpulses in Actor would do, i.e. scale by weight:
                // impulse divided by particle mass (which is 1/w):
                m_velocities[i] += particleDisplacementVector[i] * m_particles[i].w;
            }
            _particleData.SetVelocities(m_actor.indices[0], m_actor.indexCount, m_velocities);
        }

        private void SetBoneWeights(Vector3[] cachedMeshVertices, List<int> nearestVertexIndexForParticle)
        {
            Matrix4x4[] cachedBindposes = referenceAnimSkin.sharedMesh.bindposes;
            BoneWeight[] cachedBoneWeights = referenceAnimSkin.sharedMesh.boneWeights;
            // Make a WeightList-list, one for each bone in the skinned mesh
            WeightList[] boneWeights = new WeightList[referenceAnimSkin.bones.Length];
            for (int i = 0; i < referenceAnimSkin.bones.Length; i++)
            {
                boneWeights[i] = new WeightList();
                boneWeights[i].boneIndex = i;
            }
            // for every particle, add up to 4 appropriate VertexWeights based on the bones affecting the nearest vertex
            for (int particleIndex = 0; particleIndex < nearestVertexIndexForParticle.Count; particleIndex++)
            {
                int nearestVertexIndex = nearestVertexIndexForParticle[particleIndex];
                Vector3 vertexPos = cachedMeshVertices[nearestVertexIndex];
                // for each non-zero BoneWeight of that vertex, add a corresponding VertexWeight,
                // remembering the offset from the bone to the vertex (so it can be used on the particle later)
                BoneWeight bw = cachedBoneWeights[nearestVertexIndex];
                if (bw.weight0 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex0].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex0].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight0));
                }
                if (bw.weight1 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex1].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex1].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight1));
                }
                if (bw.weight2 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex2].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex2].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight2));
                }
                if (bw.weight3 != 0.0f)
                {
                    Vector3 localPt = cachedBindposes[bw.boneIndex3].MultiplyPoint3x4(vertexPos);
                    boneWeights[bw.boneIndex3].weights.Add(new ParticleBoneWeight(particleIndex, localPt, bw.weight3));
                }
            }
            particleBoneWeights = boneWeights;
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
    }
}
