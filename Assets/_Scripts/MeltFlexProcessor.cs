using System.Collections;
using System.Collections.Generic;
using uFlex;
using UnityEngine;

public class MeltFlexProcessor : FlexProcessor, IStorable
{
    //private const float MAX_MELT_FACTOR = 9.8f;
    public KeyCode m_key = KeyCode.M;
    public GameObject melt_object;

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

        if (Input.GetKeyDown(m_key))
        {
            //for (int index = 0; index < cntr.m_velocities.Length; ++index)
            //{
            //    cntr.m_velocities[index] = new Vector3(0.0f, -MAX_MELT_FACTOR, 0.0f);
            //}
            //melt_object.GetComponent<FlexAnimation>().enabled = false;
            melt_object.GetComponent<FlexShapeMatching>().enabled = false;

        }
        if (Input.GetKeyUp(m_key))
        {
            //for (int index = 0; index < cntr.m_velocities.Length; ++index)
            //{
            //    cntr.m_velocities[index] = new Vector3(0.0f, -MAX_MELT_FACTOR, 0.0f);
            //}
            //melt_object.GetComponent<FlexAnimation>().enabled = true;
            melt_object.GetComponent<FlexShapeMatching>().enabled = true;

        }
    }
}
