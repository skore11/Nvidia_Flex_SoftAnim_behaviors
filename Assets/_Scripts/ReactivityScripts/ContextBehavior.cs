using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: on Trigger enter for child, change the position of the box collider on the parent based on tags from trigger collider
//based on the tag change the position of the box collider accordingly
//TODO: perform solver sub step and iteration changes on solver parameters only on the current flex game object 
//and not for all flex game objects in the Flex container.
namespace uFlex
{
    public class ContextBehavior : FlexProcessor
    {
        public FlexSolver flexSolver;
        public FlexParameters flexParams;

        public float solverSubSteps = 1.0f;
        public float numOfIterations = 1.0f;
        // Start is called before the first frame update
        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            flexSolver.m_solverSubSteps = (int) solverSubSteps;
            flexParams.m_numIterations = (int) numOfIterations;

            //In Child objects
            //if OntriggerEnter(Coll)
            //  if (Coll.tag = "fear", "happy", "sad", "excited")
            //     Set appropriate behavior/change box collider position/change iterations or solver substeps

        }

        void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case ("Rigid"):
                    flexSolver.m_solverSubSteps = 3;
                    break;
                case ("loose"):
                    flexSolver.m_solverSubSteps = 1;
                    break;
                case ("SoftBody"):
                    flexParams.m_numIterations = 3;
                    break;
                case ("RigidBody"):
                    flexParams.m_numIterations = 20;
                    break;
                default:
                    print("Nothing to trigger");
                    break;
            }
        }

            void OnGUI()
        {
            GUI.Label(new Rect(100, 10, 120, 50), "Flex Solver Sub Steps");
            solverSubSteps = GUI.HorizontalSlider(new Rect(100, 50, 120, 50), solverSubSteps, 1.0F, 5.0F);
            GUI.Label(new Rect(100, 60, 120, 50), "Flex Parameters: Number of iterations");
            numOfIterations = GUI.HorizontalSlider(new Rect(100, 100, 120, 50), numOfIterations, 1.0F, 20.0F);
        }
    }
}

