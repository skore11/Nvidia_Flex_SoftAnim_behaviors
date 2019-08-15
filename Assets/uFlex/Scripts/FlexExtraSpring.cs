using UnityEngine;
using System.Collections;

namespace uFlex

{
    public class FlexExtraSpring : MonoBehaviour
    {

        public FlexParticles m_bodyA;
        public FlexParticles m_bodyB;

        public int m_idA = -1;
        public int m_idB = -1;

        public float m_restLength =0;
        public float m_stiffness = 1;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmosSelected()
        {
            if (m_bodyA != null & m_bodyB != null && m_idA != -1 && m_idB != -1)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(m_bodyA.m_particles[m_idA].pos, m_bodyB.m_particles[m_idB].pos);
            }
        }
    }
}