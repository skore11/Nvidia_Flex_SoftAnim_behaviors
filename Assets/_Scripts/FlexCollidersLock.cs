using UnityEngine;
using NVIDIA.Flex;
using System;

namespace Percubed.Flex
{
    /**
     * Lock particles in the flex object based on any Colliders on the same GameObject.
     * Note that FixedParticles of the flex asset are persistently stored!
     * So they will survive a restart, so we do need to reset them initially,
     * and then set/reset when the Colliders change or are activated/deactivated (by calling ReLock).
     */
    [RequireComponent(typeof(FlexActor))]
    public class FlexCollidersLock : MonoBehaviour
    {
        FlexSoftActor m_actor;
        private Vector4[] m_particles; // local cache

        void Awake()
        {
            m_actor = GetComponent<FlexSoftActor>();
        }

        void Start()
        {
            m_particles = new Vector4[m_actor.indexCount];
            m_actor.onFlexUpdate += OnFlexUpdate;
        }

        /**
         * Call this method after moving/adding/enabling/disabling colliders, to trigger re-locking.
         */
        void ReLock()
        {
            m_actor.onFlexUpdate += OnFlexUpdate;
        }

        /**
         * This will only run once by unregistering itself (until ReLock is called).
         */
        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            m_actor.onFlexUpdate -= OnFlexUpdate; // only run once!
            m_actor.asset.ClearFixedParticles();
            _particleData.GetParticles(m_actor.indices[0], m_actor.indexCount, m_particles);
            // find all particles that are inside one of the colliders, and add it to fixedParticles:
            Collider[] to_be_locked_colls = GetComponents<Collider>();
            float particleRadius = m_actor.asset.particleSpacing; // not sure if this is the radius or the diameter
            for (int i = 0; i < m_particles.Length; i++)
            {
                Collider[] overlapped_colls = Physics.OverlapSphere(m_particles[i], particleRadius);
                foreach (Collider ovrlp_c in overlapped_colls)
                {
                    if (Array.IndexOf<Collider>(to_be_locked_colls, ovrlp_c) > -1) {
                        m_actor.asset.FixedParticle(i, true);
                        break; // no need to check other colliders for this particle now
                    }
                }
            }
            m_actor.asset.Rebuild();
        }
    }
}
