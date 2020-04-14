using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
//using NVIDIA.Flex;
using uFlex;

public class FlexAgent : Agent
{	
	/// <summary>
	/// The flex academy. Contains the flex container and controls the environment.
	/// </summary>
	public FlexAcademy academy;

    /// <summary>
    /// Flex animation to match.
    /// </summary>
    public FlexAnimation flexAnim;

    /// <summary>
    /// Flex animation to match.
    /// </summary>
    public FlexParticles flexParticles;

    /// <summary>
    /// The speed. Proportional to how fast the agent can move.
    /// </summary>
    public float speed = 10;

    public bool check = false;
    
    /// <summary>
    /// Agent reset. Teleports the agent back to the center and the target to a new random position.
    /// </summary>
    //public override void AgentReset()
    //{
    //	academy.AcademyReset ();

    //	if (this.transform.position.y < -1.0)
    //	{
    //		// The agent fell
    //		GetComponent<FlexActor>().Teleport(new Vector3(0f, 0.5f, 0), Quaternion.Euler(Vector3.zero));
    //	}
    //}

    struct ImpulseInfo { public Vector3 impulse; public int particle; }
    List<ImpulseInfo> m_impulses = new List<ImpulseInfo>();


    private void Start()
    {
        print(flexParticles.m_particles.Length);
        print(flexAnim.particlePositions.Length);
        //print(flexParticles.m_particles[1].pos);
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = flexParticles.m_particles[1].pos;
        //cube.transform.localScale = new Vector3(1.25f, 1.5f, 1);
        
    }

    private void Update()
    {
        if (check)
        {
            for (int i = 0; i < flexParticles.m_particles.Length/2; i++)
            {
                flexParticles.m_particles[i].pos = flexAnim.particlePositions[i];


            }
            //flexParticles.m_particles[1].pos = flexAnim.particlePositions[1];
            //flexParticles.m_particles[2].pos = flexAnim.particlePositions[2];
            //check = false;
        }

       
        //check = true;
        //print("flex Particle POS:" + flexParticles.m_particles[1].pos);
        //print("Particle POS:" + flexAnim.particlePositions[1]);

        //

    }

  
    /// <summary>
    /// Collects observations which correspond to all the particles in the Flex GameObject (positions, masses, velocities)
    /// </summary>
    public override void CollectObservations()
	{
		//foreach (var flexObject in academy.flexContainer.m_flexGameObjects)
		//{
		//	for (int i = 0; i < flexObject.m_particles.Length; i++) 
		//	{
  //              AddVectorObs(flexObject.m_particles[i].pos);
  //  //            AddVectorObs(actor.particles [i]);
		//		//AddVectorObs(actor.velocities [i]);
		//		//AddVectorObs(actor.id);
		//	}
		//}

        for (int i = 0; i < flexParticles.m_particles.Length; i++)
        {
            
            AddVectorObs(flexParticles.m_particles[i].pos);
        }

		FillUpOberservationVectorWithDummyValue(-1.0f);
	}

	/// <summary>
	/// Computes average velocity magnitude.
	/// </summary>
	/// <param name="velocities">List of 3D velocites.</param>
	float ComputeAverageVelocityMagnitude(Vector3[] velocities)
	{
		Vector3 averageVelocity = Vector3.zero;

		foreach(Vector3 velocity in velocities)
		{
			averageVelocity.x += velocity.x;
			averageVelocity.y += velocity.y;
			averageVelocity.z += velocity.z;
		}	
		averageVelocity.x /= velocities.Length;
		averageVelocity.y /= velocities.Length;
		averageVelocity.z /= velocities.Length;

		return averageVelocity.magnitude;
	}

	/// <summary>
	/// Specifies the reward setup of the match animation task.
	/// </summary>
	void AddAgentRewards()
	{
        //// Target was pushed.
        //if (ComputeAverageVelocityMagnitude(academy.target.GetComponent<FlexActor>().velocities) > 0.1f)
        //{
        //	AddReward(1.0f);
        //}

        //// Time penalty
        //AddReward(-0.05f);

        //// Agent or target fell off platform
        //if (this.transform.position.y < -1.0 || 
        //	academy.target.transform.position.y < -1.0)
        //{
        //	AddReward(-1.0f);
        //	Done();
        //}

        
    }

    public void ApplyImpulse(Vector3 _impulse, int _particle = -1)
    {
        ImpulseInfo info;
        info.impulse = _impulse;
        info.particle = _particle;
        m_impulses.Add(info);
    }

    void CheckPos(float mag1)
    {
        
            for (int j = 0; j < flexAnim.particlePositions.Length; j++)
            {

            //float mag2 = Vector3.Magnitude(flexAnim.particlePositions[j]);
            //if (mag1 / mag2 >= 0.99)
            //{
            //    print(j);
            //}

            //print("index:" + j + "Particle local POS:" + flexParticles.m_particles[j].pos);
            //print("index:" + j + "anim local POS:" + flexAnim.particlePositions[j]);
            //Debug.DrawLine(flexParticles.m_particles[j].pos, flexAnim.particlePositions[j], Color.green);
        }
        
    }

    private int FindNearestIndex(Vector3 flexParticle, Vector3[] animParticles)
    {
        float nearestDist = 10000f;
        int nearestIndex = -1;
            for (int j = 0; j < animParticles.Length; j++)
            {
            float dist = Vector3.Distance(flexParticle, animParticles[j]);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIndex = j;
            }
        }
        return nearestIndex;
        
    }

    /// <summary>
    /// Executes the action specified by the brain.
    /// </summary>
    /// <param name="vectorAction">The float action vector.</param>
    /// <param name="textAction">The string text action.</param>
    void ExecuteAction(float[] vectorAction, string textAction)
	{
		// Actions, size = 2
		Vector3 controlSignal = Vector3.zero;
		controlSignal.x = vectorAction[0];
		controlSignal.z = vectorAction[1];
        foreach (var flexObject in academy.flexContainer.m_flexGameObjects)
        {
            for (int i = 0; i < flexObject.m_particles.Length; i++)
            {
                if (flexObject.m_particles[i].pos != flexAnim.particlePositions[i])
                {
                    ApplyImpulse(controlSignal * speed);
                }
            }
        }
        
	}

	/// <summary>
	/// Agent action. Specifies rewards and executes the action specified by the brain.
	/// </summary>
	/// <param name="vectorAction">The float action vector.</param>
	/// <param name="textAction">The string text action.</param>
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		AddAgentRewards();
		ExecuteAction(vectorAction, textAction);
	}


}
