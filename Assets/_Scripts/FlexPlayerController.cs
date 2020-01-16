using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uFlex

{

    public class FlexPlayerController : FlexProcessor
    {

        public float speed;

        public GameObject fGB;

        //[HideInInspector]
        public FlexContainer m_cntr;

        private int c;

        Particle[] a;

        //Transform temp;

        void Start()
        {
            //temp = fGB.transform;
        }

        //void Update()
        //{
        //    float moveHorizontal = Input.GetAxis("Horizontal");
        //    float moveVertical = Input.GetAxis("Vertical");
        //    float moveUpDown = Input.GetAxis("UpandDown");

        //    Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
        //    //temp.Translate(movement * speed);
        //    temp.TransformPoint(movement * speed);
        //    temp.Translate(movement * speed);
        //}

        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float moveUpDown = Input.GetAxis("UpandDown");

             c = this.GetComponent<FlexParticles>().m_particlesCount;
             a = this.GetComponent<FlexParticles>().m_particles;

            //Why does the movement not work for just a single flex game object
            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            //movement = transform.TransformDirection(movement);
            for (int i = 0; i < m_cntr.m_particlesCount; i++)
           // for (int i = 0; i < c ; i++)
            {
                m_cntr.m_particles[i].pos = m_cntr.m_particles[i].pos - movement;

                //a[i].pos = transform.TransformPoint(a[i].pos);

                //a[i].pos = a[i].pos - movement;
                //
            }

        }
    }
}