using System.Collections;
using System.Collections.Generic;
using uFlex;
using UnityEngine;

public class JiggleFlexProcessor : FlexProcessor, IStorable
{
    private const float MAX_JIGGLE_FACTOR = 20f;
    public KeyCode m_key = KeyCode.J;
    private FlexParticles fParticles;

    public void Awake()
    {
        this.fParticles = GetComponent<FlexParticles>();
    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        var particles = fParticles.m_particles;
        if (Input.GetKey(m_key))
        {
            for (int pId = 0; pId < fParticles.m_particlesCount; pId++)
            {
                particles[pId].pos -= new Vector3((Random.value - 0.5f) * MAX_JIGGLE_FACTOR, (Random.value - 0.5f) * MAX_JIGGLE_FACTOR, (Random.value - 0.5f) * MAX_JIGGLE_FACTOR) * Time.deltaTime;
            }
        }
    }
}
