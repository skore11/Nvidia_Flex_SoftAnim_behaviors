using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

public class MyFlexParticlesLock : FlexParticlesLock
{
    private FlexContainer m_cntr;
    private TriggerParent m_triggerParent;

    public FlexParticles m_Particles;

    public bool moveCol;
    private BoxCollider myCol;

    public void Awake()
    {
        m_triggerParent = FindObjectOfType<TriggerParent>();
        myCol = GetComponent<BoxCollider>();
        //moveCol = FindObjectOfType<MoveCollider>().movCol;
        //print(m_triggerParent.name);
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        
        //print("number of locked particles: " + m_lockedParticlesMasses.Count);
        m_cntr = cntr;  // cache local reference for Gizmo drawing
        if (m_triggerParent == null)
        {
            // have to potentially look for TriggerParent again, since it's added dynamically
            // TODO: rethink this logic, there are many TriggerParents, but this only looks for one!
            m_triggerParent = FindObjectOfType<TriggerParent>();
        }
        if (m_triggerParent.changeCollider || moveCol)
        {
            //print("change collider");
            // update the locked particles: first clear, then call FlexStart again
            //print("number of locked particles before clear: " + m_lockedParticlesMasses.Count);

            //this.m_lockedParticlesIds.Clear();
            /*this.*/
            this.m_lockedParticlesMasses.Clear();
            //Equate invMass to 0.0f in for loop below to accumulate locked particles
            for (int i = 0; i < m_lockedParticlesIds.Count; i++)
            {
            cntr.m_particles[m_lockedParticlesIds[i]].invMass = /*0.0f*/m_Particles.m_mass/*1.0f*/;
            }
            this.m_lockedParticlesIds.Clear();
            base.FlexStart(solver, cntr, parameters);
            //for (int i = 0; i < m_lockedParticlesMasses.Count; i++)
            //{
            //    print("After clearing, locked particle index: " + m_lockedParticlesIds[i] + " and inv. mass of each locked particle: " + m_lockedParticlesMasses[i]);
            //}
            moveCol = false;
            m_triggerParent.changeCollider = false;
        }
        base.PostContainerUpdate(solver, cntr, parameters);
    }

    private void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            myCol.center += new Vector3(0.0f, 2.0f, 0.0f);

            moveCol = true;
        }
        if (Input.GetKeyDown("k"))
        {
            myCol.center += new Vector3(0.0f, -2.0f, 0.0f);

            moveCol = true;
        }
        if (Input.GetKeyDown("j"))
        {
            myCol.center += new Vector3(2.0f, 0.0f, 0.0f);
            moveCol = true;
        }
        if (Input.GetKeyDown("l"))
        {
            myCol.center += new Vector3(-2.0f, 0.0f, 0.0f);
            moveCol = true;
        }
        if (Input.GetKeyDown("i"))
        {
            myCol.center += new Vector3(0.0f, 0.0f, 2.0f);
            moveCol = true;
        }
        if (Input.GetKeyDown("p"))
        {
            myCol.center += new Vector3(0.0f, 0.0f, -2.0f);
            moveCol = true;
        }
        if (Input.GetKeyDown("b"))
        {
            myCol.size += new Vector3(1.0f, 1.0f, 1.0f);
            moveCol = true;
        }
        if (Input.GetKeyDown("v"))
        {
            myCol.size -= new Vector3(1.0f, 1.0f, 1.0f);
            moveCol = true;
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
