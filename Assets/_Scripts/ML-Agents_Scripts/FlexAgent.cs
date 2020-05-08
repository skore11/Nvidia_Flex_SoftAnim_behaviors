using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;
//using NVIDIA.Flex;
using uFlex;



public class FlexAgent : Agent
{	
	/// <summary>
	/// The flex academy. Contains the flex container and controls the environment.
	/// </summary>
	//public Academy academy;

    /// <summary>
    /// Flex animation to match.
    /// </summary>
    public FlexAnimation flexAnim;

    /// <summary>
    /// Flex animation to match.
    /// </summary>
    public FlexParticles flexParticles;

    public FlexContainer flContainer;

    public FlexSolver flSolver;

    public FlexParameters flParams;

    private float[] temp;

    private ApplyFlexReward myflexReward;

    private AffectIterations affectIt;
    /// <summary>
    /// The speed. Proportional to how fast the agent can move.
    /// </summary>

    public bool check = false;

    public bool reset = false;

    /// <summary>
    /// Agent reset. Teleports the agent back to the center and the target to a new random position.
    /// </summary>
    public void Awake()
    {
        myflexReward = this.GetComponent<ApplyFlexReward>();
        
        //test.enabled = false;
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;

    }


    public void EnvironmentReset()
    {
        //    //if (flParams.m_numIterations > 20)
        //    //{
        //    print(flSolver);
        //    print(flContainer);
        //    
        myflexReward.iterations = Random.Range(0, 5);

        //test.FlexStart(flSolver, flContainer, flParams);
        //reset = true;
        print("reset:" + flParams.m_numIterations);
        //    //}

    }



    public void Update()
    {
        //print("update");
        //ExecuteAction(temp);
        OnActionReceived(temp);
    //    //    //
    //    test.PostContainerUpdate(flSolver, flContainer, flParams);
    //    if (flParams.m_numIterations >= 25)
    //    {
    //        EndEpisode();
    //        EnvironmentReset();
    //    }

    }


    /// <summary>
    /// Collects observations which correspond to all the particles in the Flex GameObject (positions, masses, velocities)
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(flParams.m_numIterations);
        // TODO: this does not exist anymore in the sensor class:
		//FillUpOberservationVectorWithDummyValue(-1.0f);
        // TODO: check up if filling up with dummy values is still needed or even recommended!
	}



	/// <summary>
	/// Specifies the reward setup of the match animation task.
	/// </summary>
	void AddAgentRewards()
	{
        print("Adding agent reward");


        if (myflexReward.addReward)
        {
            AddReward(1.0f);
            myflexReward.addReward = false;
        }

        if (myflexReward.addReward1)
        {
            AddReward(-0.05f);
            myflexReward.addReward1 = false;
        }
    }

    

    /// <summary>
    /// Executes the action specified by the brain.
    /// </summary>
    /// <param name="vectorAction">The float action vector.</param>
   public void ExecuteAction(float[] vectorAction)
	{
        check = true;
        print("Execute action");
        
        myflexReward.PostContainerUpdate(flSolver, flContainer, flParams);
        //could add a time limit here 
        if (flParams.m_numIterations >= 25)
        {
            print("test" + flParams.m_numIterations);
            EndEpisode();
            EnvironmentReset();
            check = false;
        }

    }

    /// <summary>
    /// Agent action. Specifies rewards and executes the action specified by the brain.
    /// </summary>
    /// <param name="vectorAction">The float action vector.</param>
    public override void OnActionReceived(float[] vectorAction)
	{
        
        print("Action received");
		AddAgentRewards();
        //temp = vectorAction;
        ExecuteAction(vectorAction);
	}


}
