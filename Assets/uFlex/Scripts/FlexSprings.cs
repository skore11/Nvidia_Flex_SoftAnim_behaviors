using UnityEngine;
using System.Collections;
using System;

namespace uFlex
{
    //[Serializable]
    //public struct FlexSpring
    //{
    //    public int idA;
    //    public int idB;
    //    public float restLength;
    //    public float stiffness;
    //}

    public class FlexSprings : MonoBehaviour
    {
     //   [HideInInspector]
        public int m_springsCount;

    //    public FlexSpring[] m_springs;

        [HideInInspector]
        public int[] m_springIndices;

        [HideInInspector]
        public float[] m_springCoefficients; 

        [HideInInspector]
        public float[] m_springRestLengths;

        public int m_springsIndex = -1;

        public bool m_overrideStiffness = false;

        public float m_newStiffness = 1.0f;


        public bool m_debug = false;

        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            if (m_overrideStiffness)
                OverrideStiffness();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_overrideStiffness)
                OverrideStiffness();
        }

        private void OverrideStiffness()
        {
            for (int i = 0; i < m_springsCount; i++)
            {
                m_springCoefficients[i] = m_newStiffness;
            }
        }

        public virtual void OnDrawGizmosSelected()
        {

            FlexParticles particles = GetComponent<FlexParticles>();



            if ( m_debug)
            {

                for (int i = 0; i < m_springsCount; i++)
                {
                    Color c = m_springCoefficients[i] > 0.5 ? Color.red : Color.green;
                    Debug.DrawLine(particles.m_particles[m_springIndices[i*2 + 0]].pos, particles.m_particles[m_springIndices[i*2 + 1]].pos, c);
                }
            }


        }
    }
}