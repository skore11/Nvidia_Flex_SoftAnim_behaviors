using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

public class MyFlexMouseDrag : FlexMouseDrag
{
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        int tmp_part_id = -1;
        if (Input.GetMouseButtonUp(0))
        {
            if (m_mouseParticle != -1)
            {
                // we got the end of a mouse drag with a particle selected,
                // notify whoever needs to know!

                //TODO: move the following to a class that is appropriately named

                //print(m_mouseParticle);
                //print(m_mousePos);
                //this.GetComponent<CreateBehavior>().behavior.dictionary.Add(m_mouseParticle, mousePos);
                SerializableMap<int, Vector3> tempIVD = new SerializableMap<int, Vector3>();
                tempIVD.Add(m_mouseParticle, m_mousePos);
                this.GetComponent<CreateBehavior>().labeledBehavior.Add("testingtesting", tempIVD);

                // remember particle id, since we need to undo parent's setting it back to non-zero mass:
                tmp_part_id = m_mouseParticle;
            }
        }
        base.PostContainerUpdate(solver, cntr, parameters); // make the base class do its thing first

        // undo what the parent did, i.e. set the mass back to 0, we want to keep it locked:
        // TODO: think about this again?
        if (tmp_part_id != -1)
        {
            cntr.m_particles[tmp_part_id].invMass = 0.0f;
        }
    }
}
