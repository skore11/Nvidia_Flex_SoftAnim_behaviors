using UnityEngine;
using System.Collections.Generic;

namespace uFlex
{
    /// <summary>
    /// Lock particles which are inside the collider
    /// WARNING. Tearable cloths will cause wrong behaviour
    /// </summary>
    public class FlexParticlesLock : FlexProcessor
    {
        public bool m_lock = false;
        public List<int> m_lockedParticlesIds = new List<int>();
        public List<float> m_lockedParticlesMasses = new List<float>();



        public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            for (int i = 0; i < cntr.m_particlesCount; i++)
            {
                Collider collider = GetComponent<Collider>();
                Collider[] colliders = Physics.OverlapSphere(cntr.m_particles[i].pos, 1.0f);
                foreach (Collider c in colliders)
                {
                    if (c == collider)
                    {
                        m_lockedParticlesIds.Add(i);
                        m_lockedParticlesMasses.Add(cntr.m_particles[i].invMass);
                        cntr.m_particles[i].invMass = 0.0f;
                    }
                }
            }
        }


        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            for (int i = 0; i < m_lockedParticlesIds.Count; i++)
            {
                if (m_lock)
                    cntr.m_particles[m_lockedParticlesIds[i]].invMass = 0.0f;
                else
                    cntr.m_particles[m_lockedParticlesIds[i]].invMass = m_lockedParticlesMasses[i];
            }
        }

        void OnDrawGizmosSelected()
        {
            //Gizmos.color = Color.red;
            //for (int i = 0; i < m_lockedParticlesIds.Count; i++)
            //{
            //    Gizmos.DrawSphere(m_cntr.m_particles[m_lockedParticlesIds[i]].pos, 0.2f);
            //}
        }
    }
}