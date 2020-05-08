using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;


public class AffectIterations : FlexProcessor
{
    public int iterations;

    public FlexAgent m_flAgent;

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        parameters.m_numIterations = iterations;
        if (m_flAgent.reset)
        {
            Reset();
            print("reset");
        }

        if (m_flAgent.check)
        {
            
            if (iterations <= 25)
            {
                print("affect:" + iterations);
                iterations += 1;
            }
        }
      
    }

    private void Reset()
    {
        iterations = Random.Range(0, 5);
    }
}

