using System.Collections;
using System.Collections.Generic;
using uFlex;
using UnityEngine;

public class JiggleFlexProcessor : FlexProcessor
{
    private const float MAX_JIGGLE_FACTOR = 1000f;
    public KeyCode m_key = KeyCode.J;

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

        if (Input.GetKeyDown(m_key))
        {
            for (int index = 0; index < cntr.m_velocities.Length; ++index)
            {
                cntr.m_velocities[index] = new Vector3((Random.value - 0.5f) * MAX_JIGGLE_FACTOR, (Random.value - 0.5f) * MAX_JIGGLE_FACTOR, (Random.value - 0.5f) * MAX_JIGGLE_FACTOR);
            }
        }
    }
}
