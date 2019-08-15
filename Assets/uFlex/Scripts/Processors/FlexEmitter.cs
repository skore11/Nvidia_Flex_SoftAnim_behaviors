using UnityEngine;
using System.Collections;


namespace uFlex

{
    /// <summary>
    /// Emits fluids from FlexParticles
    /// </summary>
    public class FlexEmitter : FlexProcessor
    {
        public FlexParticles m_particles;

        public bool m_alwaysOn = false;

        public KeyCode m_key = KeyCode.F;

        public int m_id = 0;

        public int m_rate = 10;

        public float m_speed = 0;

        public float m_radius = 1.0f;

        public bool m_loop = true;

        private Color m_color = Color.gray;

        // Use this for initialization
        void Start()
        {
            if(m_particles == null)
                m_particles = GetComponent<FlexParticles>();
        }



        public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            Vector3 vel = transform.forward * m_speed;
            Vector3 pos = transform.position;


            if (Input.GetKeyDown(m_key))
            {
                m_color = new Color(Random.value, Random.value, Random.value, 1);
            }

            if (Input.GetKey(m_key) || m_alwaysOn)
            {
                
                for (int i = m_id; i< m_id + m_rate && i < m_particles.m_particlesCount; i++)
                {

                    m_particles.m_particlesActivity[i] = true;
                    m_particles.m_particles[i].pos = pos + UnityEngine.Random.insideUnitSphere * m_radius;
                    m_particles.m_velocities[i] = vel;
                    m_particles.m_colours[i] = m_color;


                }

                m_id += m_rate;

                if (m_loop && m_id > m_particles.m_particlesCount-1)
                    m_id = 0;
            }


        }


        void OnGUI()
        {
            if(!m_alwaysOn)
                GUI.Label(new Rect(10, 25, 200, 20), m_key +" to emit particles");
        }
    }
}