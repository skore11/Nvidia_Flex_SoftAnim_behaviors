using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uFlex
{
    public class TriggerParent : MonoBehaviour //Flexprocessor
    {
        //public string[] states = { "happy", "tight", "proud", "injured", "sad" };
        [HideInInspector]
        public bool changeCollider = false;
        [HideInInspector]
        public bool changeSolverSubsteps = false;
        BoxCollider parentCol;
        //private FlexSolver m_solver;

        private void Start()
        {
            parentCol = gameObject.GetComponentInParent<BoxCollider>();
            //m_solver = FindObjectOfType<FlexSolver>();
            //Transform temp = gameObject.GetComponentInParent<Transform>();
            //parentCol.center = temp.TransformPoint(parentCol.center);
            //print(parentCol.center);
        }


        /*public bool*/
        void OnTriggerEnter(Collider other)
        {
            print(other.tag);
            print(other.name);
            //BoxCollider parentCol = gameObject.GetComponentInParent<BoxCollider>();
            
            //Transform temp = gameObject.GetComponentInParent<Transform>();
            //parentCol.center = temp.TransformPoint(parentCol.center);
            //print(parentCol.center);
            //Also include no. of iterations and solver sub steps
            switch (other.tag)
            {
                case ("happy"):
                    parentCol.center = new Vector3(-0.0523f, 16.378f, -0.9889f);
                    parentCol.size = new Vector3(1.279f, 6.9665f, 14.3607f);
                    /*return */changeCollider = true;
                    break;
                case ("sad"):
                    parentCol.center = new Vector3(-0.05160236f, 17.81393f, 6.989012f);
                    parentCol.size = new Vector3(4.167196f, 5.130798f, 7.533965f);
                    /*return */
                    changeCollider = true;
                    break;
                case ("backtight"):
                    parentCol.center = new Vector3(-0.05160236f, 21.74347f, -10.6998f);
                    parentCol.size = new Vector3(4.167196f, 4.654282f, 7.071327f);
                    /*return */
                    changeCollider = true;
                    break;
                case ("proud"):
                    parentCol.center = new Vector3(-0.05160236f, 20.61224f, -2.289454f);
                    parentCol.size = new Vector3(4.167196f, 4.868568f, 11.29964f);
                    /*return */
                    changeCollider = true;
                    break;
                case ("injured"):
                    parentCol.center = new Vector3(-2.395f, 13.3752f, 3.40045f);
                    parentCol.size = new Vector3(2.339828f, 4.43866f, 5.83067f);
                    /*return*/ changeCollider = true;
                    break;
                //case ("Rigid"):
                //    m_solver.m_solverSubSteps = 3;
                //    changeSolverSubsteps = true;
                //    break;
                //case ("loose"):
                //    m_solver.m_solverSubSteps = 1;
                //    changeSolverSubsteps = true;
                //    break;
                //case ("sad"):
                default:
                    print("Nothing to trigger");
                    /*return*/ changeCollider = false;
                    break;
            }

        }

    }

   
    }


