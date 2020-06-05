using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System;
using uFlex;
//using UnityEditor;



    public class CreateBehavior : FlexProcessor
    {
        //public Texture btnTexture;

        //private bool turnOffAnim;

        //private bool doneMoving;

        //private bool turnOnAnim;

        //private bool showAnim;    

        private int mouseParticle;

        private Vector3 mouseLocal = new Vector3();

        public InputField behaviorName;

        
    //private bool once;
        public GetBehaviors getBehaviors;
        
        //public IntVector3Dictionary behavior = new IntVector3Dictionary();
        //public IntVector3Dictionary behavior = new IntVector3Dictionary();


        //Note: below is a an implementation of a dictionary of dictionaries
        public SerializableMap<string, SerializableMap<int, Vector3>> labeledBehavior = new SerializableMap<string, SerializableMap<int, Vector3>>();

        public SerializableMap<string, SerializableMap<int, Vector3>> container = new SerializableMap<string, SerializableMap<int, Vector3>>();

    //[Serializable]
    //public class TestMap : MyMap<int, Vector3> { }
    //public TestMap testMap;
    //public MyMap<int, Vector3> mybehavior = new MyMap<int, Vector3>();

    //public InputField myinputfield;
        public FlyCamera flyCam;
        private Vector3 reset;
        public bool flyCamEnable = false;
        private FlexPlayerController playerController;

        private UIController m_UIController;

        private FileStream file;
        void Start()
        {
             //flyCam = FindObjectOfType<FlyCamera>();
            m_UIController = FindObjectOfType<UIController>();
            playerController = FindObjectOfType<FlexPlayerController>();
            //reset = flyCam.resetTransform;
            //print(flyCam.resetTransform);
        }


        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

            //Note: might need to use a Coroutine instead!

            //Stop flex animation first then track particle positions for this object;
            if (m_UIController.turnOffAnim)
            {
                flyCamEnable = true;
                //List<Vector3> templist = null;
                //Vector3 temp;
                this.GetComponent<FlexAnimation>().enabled = false;//might have to move this to start to optimize
                flyCam.enabled = true;
                playerController.enabled = false;
                //StartCoroutine(MoveParticle());
                //int x = this.GetComponent<FlexMouseDrag>().m_mouseParticle;


                if (m_UIController.doneMoving)
                {
                m_UIController.turnOffAnim = false;
                m_UIController.doneMoving = false;
                    
                    //print(reset.position);
                    playerController.enabled = true;
                }
                
            }

            if (m_UIController.turnOnAnim)
            {
                
                //Note: need to save the behavior as an asset possibly or JSON?, need to ask stefan.
                //string jsonData = JSONSerializer.ToJson<Dictionary<int, Vector3>>(behavior);
                //File.WriteAllText(Application.persistentDataPath + "/test.json", jsonData);

                // test C# way of serializing:
                
                XmlSerializer xmlserializer = new XmlSerializer(labeledBehavior.GetType());
            //FileStream file = File.Open( "C:/Users/siciit/AppData/LocalLow/DefaultCompany/Nvidia_softAnim_trials/labbehaviorTrial2.xml", FileMode.Open, FileAccess.Write);
            //FileStream file = File.Open(Application.persistentDataPath + "/labbehaviorTrial2.xml", FileMode.Open, FileAccess.Read);
            if (!System.IO.File.Exists(Application.persistentDataPath + "/labbehaviorTrial3.xml"))
            {
               /* FileStream*/ file = File.Open(Application.persistentDataPath + "/labbehaviorTrial3.xml", FileMode.OpenOrCreate, FileAccess.Write);
            }
            else
            {
                /*FileStream */file = File.Open(Application.persistentDataPath + "/labbehaviorTrial3.xml", FileMode.Open, FileAccess.Write);
            }
                
                xmlserializer.Serialize(file, labeledBehavior);
                file.Flush();
                file.Close();
                flyCamEnable = false;
            //JSONSerializer.Save<Dictionary<int, Vector3>>("test", behavior);
            //if (behaviorName != null)
            //    {
            //        print(behaviorName.text);
            //    }
                this.GetComponent<FlexAnimation>().enabled = true;
            m_UIController.turnOnAnim = false;
          
            }

            if (m_UIController.showAnim)
            {
            m_UIController.showAnim = false;
            XmlSerializer readSerialize = new XmlSerializer(typeof(SerializableMap<string, SerializableMap<int, Vector3>>));
            //FileStream file = new FileStream(Application.persistentDataPath + "/labbehaviorTrial2.xml", FileMode.Open);
            ///*FileStream */file = File.Open("C:/Users/siciit/AppData/LocalLow/DefaultCompany/Nvidia_softAnim_trials/labbehaviorTrial2.xml", FileMode.Open, FileAccess.Read);
            /*FileStream*/ file = File.Open(Application.persistentDataPath + "/labbehaviorTrial3.xml", FileMode.Open, FileAccess.Read);
            print(file.Name);
            container = readSerialize.Deserialize(file) as SerializableMap<string, SerializableMap<int, Vector3>>;
            foreach (var var in container)
            {
                print(var.Key);
                print(var.Value);
            }


            //this.GetComponent<GetBehaviors>().localContainer = container;
            //this.GetComponent<GetBehaviors>().gotXML = true;

            getBehaviors.localContainer = container;
            getBehaviors.gotXML = true;

            //FindObjectOfType<GetBehaviors> ().localContainer = container;
            //FindObjectOfType<GetBehaviors>().gotXML = true;
            file.Flush();
            file.Close();
            }
            //store list ofvectors that have changed positions;
        }

        private void OnDisable()
        {
            //print(Application.persistentDataPath);
            //BinaryFormatter newbf = new BinaryFormatter();
            //FileStream file = File.Open(Application.persistentDataPath + "/behaviorTrial2.dat", FileMode.OpenOrCreate);


            //newbf.Serialize(file, behavior);
            //file.Close();

            //XmlSerializer serializer = new XmlSerializer(typeof(MyMap<int, Vector3>));
            //StreamWriter writer = new StreamWriter("behaviorTrial.xml");
            //serializer.Serialize(writer.BaseStream, testMap);
            //writer.Close();


            //XmlFix.SerializeXmlToFile(behavior, "test");
            //TODO: make IntVector3Dictionary a serializable generic class
            
        }

        //void PrintContents(IntVector3Dictionary dict)
        //{
        //    foreach (var x in dict)
        //    {
        //        int a = x.Key;
        //        Vector3 A = x.Value;
        //        print("Index: " + a + " Corresponding Vector: " + A);
        //    }
        //}

        //void AddContents(int a, Vector3 x)
        //{
        //    behavior.Add(a, x);
        //}


        //void OnGUI()
        //{
        //    if (!btnTexture)
        //    {
        //        //Debug.LogError("Please assign a texture on the inspector");
        //        return;
        //    }

        //    if (GUI.Button(new Rect(10, 10, 100, 30), "Move particle"))
        //    {
        //       // Debug.Log("Clicked the button with text");
        //        turnOffAnim = true;
        //    }

        //    if (GUI.Button(new Rect(10, 70, 100, 30), "Done Moving"))
        //    {
        //        //Debug.Log("Clicked the button with text");
        //        doneMoving = true;
        //    }

        //    if (GUI.Button(new Rect(10, 150, 100, 30), "Set  behavior"))
        //    {
        //        //Debug.Log("Clicked the button with text");
        //        turnOnAnim = true;
        //    }

        //    if (GUI.Button(new Rect(10, 250, 100, 30), "Show behaviors"))
        //    {
        //        //Debug.Log("Clicked the button with text");
        //        showAnim = true;
        //    }
    }

    

    
