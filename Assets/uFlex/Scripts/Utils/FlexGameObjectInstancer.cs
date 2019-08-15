using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uFlex
{
    public class FlexGameObjectInstancer : MonoBehaviour
    {
        public FlexParticles m_flexPrefab;
        public KeyCode m_key = KeyCode.C;
        public Vector3 m_initialVel;

        private List<FlexParticles> fps = new List<FlexParticles>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(m_key))
            {
                FlexParticles fp =  (FlexParticles)Instantiate(m_flexPrefab, transform.position, transform.rotation);
                fp.m_initialVelocity = fp.transform.TransformDirection(m_initialVel);
                fps.Add(fp);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {

                foreach(FlexParticles fp in fps)
                {
                    Destroy(fp.gameObject);
                }

                fps.Clear();
            }
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 200, 20), m_key +" to fire a flex body");
        }
    }
}