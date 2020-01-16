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
        BoxCollider parentCol;

        private void Start()
        {
            parentCol = gameObject.GetComponentInParent<BoxCollider>();
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
                //case ("happy"):
                //    parentCol.center = new Vector3(-0.0523f, 16.378f, -0.9889f);
                //    parentCol.size = new Vector3(1.279f, 6.9665f, 14.3607f);
                //    /*return */changeCollider = true;
                //    break;
                //case ("tight"):
                //case ("proud"):
                case ("injured"):
                    parentCol.center = new Vector3(-2.395f, 13.3752f, 3.40045f);
                    parentCol.size = new Vector3(2.339828f, 4.43866f, 5.83067f);
                    /*return*/ changeCollider = true;
                    break;
                //case ("sad"):
                default:
                    print("Nothing to trigger");
                    /*return*/ changeCollider = false;
                    break;
            }

        }

    }

   
    }


