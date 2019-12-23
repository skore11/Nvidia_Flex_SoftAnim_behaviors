using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace uFlex

{
    /// <summary>
    /// Drag particles using mouse
    /// </summary>
    [ExecuteInEditMode]
    public class FlexSelectParticles : MonoBehaviour
    {
        public FlexParticles m_flexParticles;

        private int m_mouseParticle = -1;

        private float m_mouseMass = 0;

        private float m_mouseT = 0;

        private Vector3 m_mousePos = new Vector3();

        public List<int> m_lockedParticlesIds = new List<int>();

        public List<float> m_lockedParticlesMasses = new List<float>();

        public bool m_lock = false;

        public FlexContainer cntr;

        public FlexParameters parameters;

        void OnEnable()
        {
            if (m_flexParticles == null)
                m_flexParticles = GetComponent<FlexParticles>();

            //if (Event.current.type == EventType.MouseDown)
            //{
            //    Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
            //    m_mouseParticle = PickParticle(ray.origin, ray.direction, cntr.m_particles, cntr.m_phases, cntr.m_particlesCount, parameters.m_radius * 0.8f, ref m_mouseT);
            //    RaycastHit hit = new RaycastHit();
            //    if (Physics.Raycast(ray, out hit, 1000.0f))
            //    {
            //        Debug.Log(Event.current.mousePosition);

            //        Gizmos.DrawSphere(hit.point, 2);
            //    }
            //}

        }

        

        //public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        //{

        //    for (int i = 0; i < cntr.m_particlesCount; i++)
        //    {
        //    }

        //        if (Input.GetMouseButtonDown(0))
        //    {
        //        // need up to date positions
        //        //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
        //        //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

        //        Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
        //        m_mouseParticle = PickParticle(ray.origin, ray.direction, cntr.m_particles, cntr.m_phases, cntr.m_particlesCount, parameters.m_radius * 0.8f, ref m_mouseT);

        //        if (m_mouseParticle != -1)
        //        {
        //            Debug.Log("picked: " + m_mouseParticle);

        //            m_mousePos = ray.origin + ray.direction * m_mouseT;
        //            Gizmos.DrawSphere(m_mousePos,2);
        //            m_mouseMass = cntr.m_particles[m_mouseParticle].invMass;
        //            cntr.m_particles[m_mouseParticle].invMass = 0.0f;

                
        //        }
               
        //    }
        //}


        //public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        //{
            

        //        for (int i = 0; i < m_lockedParticlesIds.Count; i++)
        //        {
        //            if (m_lock)
        //                cntr.m_particles[m_lockedParticlesIds[i]].invMass = 0.0f;
        //            else
        //                cntr.m_particles[m_lockedParticlesIds[i]].invMass = m_lockedParticlesMasses[i];
        //        }
            

        //    if (Input.GetMouseButtonUp(0))
        //    {
        //        if (m_mouseParticle != -1)
        //        {

        //            cntr.m_particles[m_mouseParticle].invMass = m_mouseMass;
        //            m_mouseParticle = -1;

        //            // need to update positions straight away otherwise particle might be left with increased mass
        //            //       Flex.SetParticles(m_solverPtr, m_cntr.m_particles, m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);

        //        }
        //    }

        //    if (m_mouseParticle != -1)
        //    {
        //        //    Flex.GetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);
        //        //    Flex.GetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_maxParticlesCount, Flex.Memory.eFlexMemoryHost);

        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        m_mousePos = ray.origin + ray.direction * m_mouseT;

        //        Vector3 pos = cntr.m_particles[m_mouseParticle].pos;
        //        Vector3 p = Vector3.Lerp(pos, m_mousePos, 0.8f);
        //        Vector3 delta = p - pos;

        //        cntr.m_particles[m_mouseParticle].pos = p;
        //        cntr.m_velocities[m_mouseParticle] = delta / Time.fixedTime;

        //        //    Flex.SetParticles(m_solverPtr, m_cntr.m_particlesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);
        //        //    Flex.SetVelocities(m_solverPtr, m_cntr.m_velocitiesHndl.AddrOfPinnedObject(), m_cntr.m_particlesCount, Flex.Memory.eFlexMemoryHost);

        //    }
        //}

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