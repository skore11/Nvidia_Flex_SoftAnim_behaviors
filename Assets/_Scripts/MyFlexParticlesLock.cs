using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

public class MyFlexParticlesLock : FlexParticlesLock
{
    private FlexContainer m_cntr;
    private TriggerParent m_triggerParent;

    public void Awake()
    {
        m_triggerParent = FindObjectOfType<TriggerParent>();
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        m_cntr = cntr;  // cache local reference for Gizmo drawing
        if (m_triggerParent == null)
        {
            // have to potentially look for TriggerParent again, since it's added dynamically
            // TODO: rethink this logic, there are many TriggerParents, but this only looks for one!
            m_triggerParent = FindObjectOfType<TriggerParent>();
        }
        if (m_triggerParent.changeCollider)
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
            m_triggerParent.changeCollider = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (m_cntr != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < m_lockedParticlesIds.Count; i++)
            {
                Gizmos.DrawSphere(m_cntr.m_particles[m_lockedParticlesIds[i]].pos, radius: 0.2f);
            }
        }
    }
}
