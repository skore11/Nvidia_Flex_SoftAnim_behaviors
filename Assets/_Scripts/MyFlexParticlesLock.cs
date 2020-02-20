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
            // update the locked particles: first clear, then call FlexStart again
            this.m_lockedParticlesIds.Clear();
            this.m_lockedParticlesMasses.Clear();
            base.FlexStart(solver, cntr, parameters);
            m_triggerParent.changeCollider = false;
        }
        base.PostContainerUpdate(solver, cntr, parameters);
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
