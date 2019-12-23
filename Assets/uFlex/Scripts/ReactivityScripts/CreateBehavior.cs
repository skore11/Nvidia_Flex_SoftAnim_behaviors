using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;


namespace uFlex
{

    public class CreateBehavior : FlexProcessor
    {
        public Texture btnTexture;

        private bool turnOffAnim;

        private bool turnOnAnim;

        //private bool once;

        //Note: create an asset of this dictionary that pertains only to this animated object
        private Dictionary<string, List<Vector3>> behavior;
        //private Dictionary<int, Vector3> behavior;

        //public InputField myinputfield;

        //private BehaviorAsset behaviorAsset;

        //private Particle[] initialPos;

        //private Particle[] newPos;

        //private Vector3[] posChanges;

        //Use a temp Vector3[] to store the changes in particle positions to be used for the behaviors

        //void Start()
        //{
        //    once = true;
        //    string behaviorName = myinputfield.text;
        //}


        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            //newPos = cntr.m_particles;
            

            //Stop flex animation first then track particle positions for this object;
            while (turnOffAnim)
            {
                
                List<Vector3> templist = null;
                //Vector3 temp;
                this.GetComponent<FlexAnimation>().enabled = false;//might have to move this to start to optimize
                //StartCoroutine(MoveParticle());
                int x = this.GetComponent<FlexMouseDrag>().m_mouseParticle;
                if (x != -1)
                {
                    
                    Vector3 worldPos = this.GetComponent<FlexParticles>().m_particles[x].pos;//might have to move this to start to optimize
                    Vector3 localPos = gameObject.transform.InverseTransformPoint(worldPos);
                    print(localPos);
                    print("mouse particle no: " + x + "corresponding local coordinate position: " + localPos);
                    templist.Add(localPos);

                }
                
                turnOffAnim = false;
            }



            if (turnOnAnim)
            {
                print("ok");

                this.GetComponent<FlexAnimation>().enabled = true;
                turnOnAnim = false;
          
            }
            //store list ofvectors that have changed positions;
        }

     

        void OnGUI()
        {
            if (!btnTexture)
            {
                Debug.LogError("Please assign a texture on the inspector");
                return;
            }

            if (GUI.Button(new Rect(10, 10, 50, 50), "Move particle"))
            {
                Debug.Log("Clicked the button with text");
                turnOffAnim = true;
            }

            if (GUI.Button(new Rect(10, 70, 50, 30), "Set  behavior"))
            {
                Debug.Log("Clicked the button with text");
                turnOnAnim = true;
            }
        }

    }

    }
