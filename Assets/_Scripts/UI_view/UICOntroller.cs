using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uFlex;

public class UIController : FlexProcessor//need to extend an Interface for a View of the UI that is an interface, This should be the main portion fo the MVC
{
    private FlexParticles flParticles;

    private ContextBehavior m_contextBehavior;

    private CreateBehavior m_createBehavior;

    private MyFlexMouseDrag m_FlexParticleLocker;

    public float solverSubSteps = 1.0f;
    public float numOfIterations = 1.0f;

    Dropdown m_Dropdown;

    public Texture btnTexture;

    public bool turnOffAnim;

    public bool doneMoving;

    public bool turnOnAnim;

    public bool showAnim;

    public InputField m_behaviorName;

    public SerializableMap<int, Vector3> tempIVD;

    void Start()
    {
        tempIVD = new SerializableMap<int, Vector3>();
        m_FlexParticleLocker = FindObjectOfType<MyFlexMouseDrag>();
        m_contextBehavior = FindObjectOfType<ContextBehavior>();
        m_createBehavior = FindObjectOfType<CreateBehavior>();
        m_Dropdown = FindObjectOfType<Dropdown>().GetComponent<Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate { m_contextBehavior.DeformCharacter(m_Dropdown); });
        //m_Deform.AddListener(delegate { deformParticle(flexCont, pInd, pVect); });
        //m_Dropdown.onValueChanged.AddListener(delegate { assignDeform = true; PostContainerUpdate(flexSolver, flexCont, flexParams); });

    }

    public override void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

    }

    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {
        solver.m_solverSubSteps = (int)solverSubSteps;
        parameters.m_numIterations = (int)numOfIterations;

        if (m_FlexParticleLocker != null)
        {
            if (m_FlexParticleLocker.picked)
            {
                //TODO: check if the labeled behavior already contains a behavior and append to that behavior
                if (m_createBehavior.labeledBehavior.Count == 0)
                {
                    print("No behaviors in Serilazable Map");
                    tempIVD = new SerializableMap<int, Vector3>();
                    tempIVD.Add(m_FlexParticleLocker.pMouseParticleID, m_FlexParticleLocker.pMouseParticlePos);
                    m_createBehavior.labeledBehavior.Add(m_behaviorName.text, tempIVD);
                }
                else
                {
                    print("There are behaviors");
                    foreach (var index in m_createBehavior.labeledBehavior)
                    {
                        if (m_behaviorName.text == index.Key)
                        {
                            foreach (var behavior in m_createBehavior.labeledBehavior.Values)
                            {
                                print(behavior.Keys);
                                behavior.Add(m_FlexParticleLocker.pMouseParticleID, m_FlexParticleLocker.pMouseParticlePos);
                                //m_createBehavior.labeledBehavior.Add(m_behaviorName.text, behavior);

                            }
                        }

                    }

                }
                //this.GetComponent<CreateBehavior>().labeledBehavior.Add(this.GetComponent<CreateBehavior>().behaviorName.text, tempIVD);
                m_FlexParticleLocker.picked = false;
            }
        }
        
        else
        {
            base.PostContainerUpdate(solver, cntr, parameters);
        }

    }

    //public static void addListerner()
    //{

    //}

    void OnGUI()
    {
        //Also add a feature to lock particles in place or drag them around

        GUIStyle myStyle = new GUIStyle();
        myStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(150, 10, 120, 50), "Flex Solver Sub Steps", myStyle);
        solverSubSteps = GUI.HorizontalSlider(new Rect(100, 50, 120, 50), solverSubSteps, 1.0F, 5.0F);
        GUI.Label(new Rect(150, 60, 120, 50), "Flex Parameters: Number of iterations", myStyle);
        numOfIterations = GUI.HorizontalSlider(new Rect(100, 100, 120, 50), numOfIterations, 1.0F, 20.0F);

        if (!btnTexture)
        {
            //Debug.LogError("Please assign a texture on the inspector");
            return;
        }

        if (GUI.Button(new Rect(10, 10, 100, 30), "Move particle"))
        {
             Debug.Log("Clicked the button with text");
            turnOffAnim = true;
        }

        if (GUI.Button(new Rect(10, 70, 100, 30), "Done Moving"))
        {
            //Debug.Log("Clicked the button with text");
            doneMoving = true;
        }

        if (GUI.Button(new Rect(10, 150, 100, 30), "Set  behavior"))
        {
            //Debug.Log("Clicked the button with text");
            turnOnAnim = true;
        }

        if (GUI.Button(new Rect(10, 250, 100, 30), "Show behaviors"))
        {
            //Debug.Log("Clicked the button with text");
            showAnim = true;
        }
    }
}


    
