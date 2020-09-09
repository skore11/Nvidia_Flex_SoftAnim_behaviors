using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace NVIDIA.Flex
{
    public class FlexNewLock : MonoBehaviour
    {
        FlexSoftActor m_actor;
        BoxCollider locked_box;
        private Vector4[] m_particles;
        // Start is called before the first frame update
        private void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
            locked_box = GetComponent<BoxCollider>();
        }
        void Start()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
            m_actor.asset.ClearFixedParticles();
            m_particles = new Vector4[m_actor.indexCount];
        }

        // Update is called once per frame
        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            
            if (locked_box != null)
            {
                _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
                //Debug.Log("test");

                // find all particles that are inside the box, and add it to the fixedParticles list:
                for (int i = 0; i < m_actor.indexCount; i++)
                {
                    Collider[] collider = GetComponents<Collider>();
                    Collider[] colliders = Physics.OverlapSphere(m_particles[i], 1.0f);
                    foreach (Collider c in colliders)
                    {
                        foreach (Collider col in collider)
                        {
                            if (c == col)
                            {
                                m_actor.asset.FixedParticle(i, true);
                                //m_lockedParticlesMasses.Add(cntr.m_particles[i].invMass);
                                //cntr.m_particles[i].invMass = 0.0f;
                            }
                        }

                    }
                }
                m_actor.asset.Rebuild();
                locked_box = null;
            }
        }
    }
}
