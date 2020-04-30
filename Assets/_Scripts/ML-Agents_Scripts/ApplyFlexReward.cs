using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

public class ApplyFlexReward : FlexProcessor
{
    //[HideInInspector]
    public bool addReward = false;

    //[HideInInspector]
    public bool addReward1 = false;

    public int iterations;

    public FlexAgent flAgent;
    // Start is called before the first frame update
    //public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    //{

    //    parameters.m_numIterations = Random.Range(0, 5);
    //    //iterations = parameters.m_numIterations;
    //    //base.FlexStart(solver, cntr, parameters);
    //    print("test in Env reset" + parameters.m_numIterations);
    //}

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        if (flAgent.check)
        {
            parameters.m_numIterations = iterations;
            //base.PostContainerUpdate(solver, cntr, parameters);
            if (parameters.m_numIterations > 15 & parameters.m_numIterations < 20)
            {
                //AddReward(1.0f);
                addReward = true;
            }

            if (parameters.m_numIterations < 15)
            {
                //AddReward(-0.05f);
                addReward1 = true;
                parameters.m_numIterations += 1;
            }

            if (parameters.m_numIterations > 20)
            {
                //AddReward(-0.05f);
                addReward1 = true;
                parameters.m_numIterations -= 1;
            }


            
        }
        
    }
}
