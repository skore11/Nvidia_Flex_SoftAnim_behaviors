using UnityEngine;
using System.Collections;

namespace uFlex

{

    public class FlexPlayerController : FlexProcessor
    {

        public float speed;

        public GameObject fGB;

        //[HideInInspector]
        public FlexContainer m_cntr;

        void Start()
        {

        }

        void Update()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float moveUpDown = Input.GetAxis("UpandDown");

            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            fGB.transform.Translate(movement * speed);


        }

        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float moveUpDown = Input.GetAxis("UpandDown");

            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            for (int i = 0; i < m_cntr.m_particlesCount; i++)
            {
                m_cntr.m_particles[i].pos = m_cntr.m_particles[i].pos - movement; 
                
            }

        }
    }
}