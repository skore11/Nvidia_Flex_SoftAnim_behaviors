using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
        public FlexContainer flexCont;

        private int pInd;
        private Vector3 pVect;

        public float solverSubSteps = 1.0f;
        public float numOfIterations = 1.0f;

        Dropdown m_Dropdown;

        UnityEvent m_Deform = new UnityEvent();

        bool assignDeform;

        [HideInInspector]
        public SerializableMap<string, SerializableMap<int, Vector3>> localBehavior;

        void Start()
        {
            m_Dropdown = FindObjectOfType<Dropdown>().GetComponent<Dropdown>();
            m_Dropdown.onValueChanged.AddListener(delegate { DeformCharacter(m_Dropdown); });
            //m_Deform.AddListener(delegate { deformParticle(flexCont, pInd, pVect); });
            //m_Dropdown.onValueChanged.AddListener(delegate { assignDeform = true; PostContainerUpdate(flexSolver, flexCont, flexParams); });

        }

        // Start is called before the first frame update
        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {
            //flexSolver.m_solverSubSteps = (int) solverSubSteps;
            //flexParams.m_numIterations = (int) numOfIterations;

            solver.m_solverSubSteps = (int)solverSubSteps;
            parameters.m_numIterations = (int)numOfIterations/2;

            //In Child objects
            //if OntriggerEnter(Coll)
            //  if (Coll.tag = "fear", "happy", "sad", "excited")
            //     Set appropriate behavior/change box collider position/change iterations or solver substeps
            if (assignDeform)
            {
                //print(pInd);
                //print(pVect);
                deformParticle(flexCont, pInd, pVect);
                assignDeform = false;
            }
            
        }

        void DeformCharacter(Dropdown change)
        {
            //print(change.options[change.value].text);
            //TODO: probably need a new class to apply the deformations to the character using the dictionary particles
            //for now just creating a function that does that
            foreach (var i in localBehavior[change.options[change.value].text])
            {
                //print(i.Key);
                //print(i.Value);
                pInd = i.Key;
                pVect = i.Value;
                assignDeform = true;  
            }
            

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
                case ("test"):
                    print("test object");
                    assignDeform = true;
                    //TODO: memorize this deformation behavior for this trigger context: perhaps another dictionary of tags (string)-><int, Vector3>
                    break;
                default:
                    print("Nothing to trigger");
                    break;
            }
        }

            void OnGUI()
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(150, 10, 120, 50), "Flex Solver Sub Steps", myStyle);
            solverSubSteps = GUI.HorizontalSlider(new Rect(100, 50, 120, 50), solverSubSteps, 1.0F, 5.0F);
            GUI.Label(new Rect(150, 60, 120, 50), "Flex Parameters: Number of iterations",myStyle);
            numOfIterations = GUI.HorizontalSlider(new Rect(100, 100, 120, 50), numOfIterations, 1.0F, 20.0F);
        }

        void deformParticle(FlexContainer cntr, int pIndex, Vector3 pPos)
        {
            cntr.m_particles[pIndex].invMass = 0.0f;
            cntr.m_particles[pIndex].pos = pPos;
            
            print("works kinda");
        }
    }
}

