using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uFlex;

public class MatchAnim: FlexProcessor
{
    public FlexParticles flParticles;

    public FlexAnimation flAnim;

    private Vector3 velAccumFlex;

    private Vector3 velAccumAnim;

    private Vector3 velFirst = new Vector3();

    private bool firstRun = true;

    //Vector3 tempFirst = new Vector3();

    Vector3[] temp = new Vector3[1271];
    //public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    //{
    //    print(Time.fixedTime);
    //    print(flAnim.particlePositions.Length);
    //    for (int i = 0; i < flAnim.particlePositions.Length; i++)
    //    {
    //        print("index: " + i + "anim particle velocity at start: " + flAnim.particlePositions[i] / Time.fixedTime);
    //    }
    //}



    // Start is called before the first frame update
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

        //for (int i = 0; i < flAnim.particlePositions.Length; i++)
        //{
        //    Debug.DrawLine(flAnim.flexParticles.m_particles[i].pos, flAnim.vertMapAsset.particleRestPositions[i], Color.green);
        //}

        if (firstRun)
        {
            firstRun = false;

            for (int i = 0; i < flAnim.particlePositions.Length; i++)
            {
                //print("index: " + i + " anim particle pos at post containter update: " + flAnim.particlePositions[i]);

                //print("index: " + i + " velocity: " + flParticles.m_velocities[i]);

                //print("temp" + temp[i]);
                //print("time. Delta time: " + Time.deltaTime);
                Vector3 delta = new Vector3();

                delta = Vector3.zero;
                velFirst = delta / Time.deltaTime;
                //print("first anim particle: " + flAnim.particlePositions[i]);
                //print("delta: " + delta);
                //print("vel: " + velFirst);
                temp[i] = flAnim.particlePositions[i];
            }

        }
        else
        {
            for (int i = 0; i < flAnim.particlePositions.Length; i++)
            {
                //print("index: " + i + " anim particle pos at post containter update: " + flAnim.particlePositions[i]);

                //print("index: " + i + " velocity: " + flParticles.m_velocities[i]);

                //print("temp" + temp[i]);
                //print("time. Delta time: " + Time.deltaTime);
                Vector3 delta = new Vector3();
                Vector3 vel = new Vector3();
                delta = (flAnim.particlePositions[i] - temp[i]);
                vel = delta / Time.deltaTime;
                //vel = vel / flAnim.particlePositions.Length;

                //print("first anim particle: " + flAnim.particlePositions[i]);
                //print("delta: " + delta);
                //print("vel: " + vel);
                velAccumAnim += vel;
                temp[i] = flAnim.particlePositions[i];

            }
        }

        print("Avg. velocity of animPaticles: " + (velAccumAnim + velFirst).magnitude / flAnim.particlePositions.Length);

        for (int i = 0; i < flParticles.m_velocities.Length; i++)
        {
            //print(flParticles.m_initialVelocity);
            //print("index: " + i + "velocity: " + flParticles.m_velocities[i]);
            flParticles.m_velocities[i] = flParticles.m_velocities[i] /*/ flParticles.m_velocities.Length*/;
            velAccumFlex += flParticles.m_velocities[i];
        }

        print("Avg. velocity of flexPaticles: " + velAccumFlex.magnitude / flParticles.m_velocities.Length);
    }

    Vector3 calcInitVel(List<Vector3> restPositons)
    {
        Vector3 initialVelocity = new Vector3();

        return initialVelocity;
    }
}
