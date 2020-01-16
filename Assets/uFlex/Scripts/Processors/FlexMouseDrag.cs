using UnityEngine;
using System.Collections;

namespace uFlex

{
    /// <summary>
    /// Drag particles using mouse
    /// </summary>
    public class FlexMouseDrag : FlexProcessor
    {
        public int m_mouseParticle = -1; // made public to allow use in subclass! --strank

        private float m_mouseMass = 0;

        private float m_mouseT = 0;

        public Vector3 m_mousePos = new Vector3(); // made public to allow use in subclass! --strank

        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // need up to date positions
            //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
            //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                m_mouseParticle = PickParticle(ray.origin, ray.direction, cntr.m_particles, cntr.m_phases, cntr.m_particlesCount, parameters.m_radius * 0.8f, ref m_mouseT);

                if (m_mouseParticle != -1)
                {
                    Debug.Log("picked: " + m_mouseParticle);

                    m_mousePos = ray.origin + ray.direction * m_mouseT;
                    m_mouseMass = cntr.m_particles[m_mouseParticle].invMass;
                    cntr.m_particles[m_mouseParticle].invMass = 0.0f;

                    //     Flex.SetParticles(m_solverPtr, m_cntr.m_particles, m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);

                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_mouseParticle != -1)
                {

                    cntr.m_particles[m_mouseParticle].invMass = m_mouseMass;
                    m_mouseParticle = -1;

                    // need to update positions straight away otherwise particle might be left with increased mass
                    //       Flex.SetParticles(m_solverPtr, m_cntr.m_particles, m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);

                }
            }

            if (m_mouseParticle != -1)
            {
                //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
                //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                m_mousePos = ray.origin + ray.direction * m_mouseT;

                Vector3 pos = cntr.m_particles[m_mouseParticle].pos;
                Vector3 p = Vector3.Lerp(pos, m_mousePos, 0.8f);
                Vector3 delta = p - pos;

                cntr.m_particles[m_mouseParticle].pos = p;
                cntr.m_velocities[m_mouseParticle] = delta / Time.fixedTime;

                //    Flex.SetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);
                //    Flex.SetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);

            }
        }

        // finds the closest particle to a view ray
        int PickParticle(Vector3 origin, Vector3 dir, Particle[] particles, int[] phases, int n, float radius, ref float t)
        {
            float maxDistSq = radius * radius;
            float minT = float.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < n; ++i)
            {
                if ((phases[i] & (int)Flex.Phase.eFlexPhaseFluid) != 0)
                    continue;

                Vector3 p = particles[i].pos;
                Vector3 delta = p - origin;
                float tt = Vector3.Dot(delta, dir);

                if (tt > 0.0f)
                {
                    Vector3 perp = delta - tt * dir;

                    float dSq = perp.sqrMagnitude;

                    if (dSq < maxDistSq && tt < minT)
                    {
                        minT = tt;
                        minIndex = i;
                    }
                }
            }

            t = minT;

            return minIndex;
        }
    }
}