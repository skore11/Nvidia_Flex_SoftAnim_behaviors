using UnityEngine;
using NVIDIA.Flex;

namespace Percubed.Flex
{
    /**
     * Lock particles in the flex object based on a BoxCollider.
     * FixedParticles of the flex asset are persistently stored!
     * So they need to only be set/reset when the BoxCollider
     * is moved or activated/deactivated.
     * TODO: maybe add an update method that checks for changes in the BoxCollider
     */
    public class FlexNewLock : MonoBehaviour
    {
        FlexSoftActor m_actor;
        BoxCollider locked_box;
        private Vector4[] m_particles;

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
