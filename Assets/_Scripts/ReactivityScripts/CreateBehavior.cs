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
        public Texture btnTexture;

        private bool turnOffAnim;

        private bool doneMoving;

        private bool turnOnAnim;

        private int mouseParticle;

        private Vector3 mouseLocal = new Vector3();

        public InputField behaviorName;

        //private bool once;

        
        //public IntVector3Dictionary behavior = new IntVector3Dictionary();
        //public IntVector3Dictionary behavior = new IntVector3Dictionary();


        //Note: below is a an implementation of a dictionary of dictionaries
        public SerializableMap<string, SerializableMap<int, Vector3>> labeledBehavior = new SerializableMap<string, SerializableMap<int, Vector3>>();
        //[Serializable]
        //public class TestMap : MyMap<int, Vector3> { }
        //public TestMap testMap;
        //public MyMap<int, Vector3> mybehavior = new MyMap<int, Vector3>();

        //public InputField myinputfield;
        public FlyCamera flyCam;
        private Transform reset;
        public bool flyCamEnable = false;
        private FlexPlayerController playerController;



        void Start()
        {
             //flyCam = FindObjectOfType<FlyCamera>();
            playerController = FindObjectOfType<FlexPlayerController>();
            //reset = flyCam.resetTransform;
            print(reset.position);
        }


        public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

            //Note: might need to use a Coroutine instead!

            //Stop flex animation first then track particle positions for this object;
            if (turnOffAnim)
            {
                flyCamEnable = true;
                //List<Vector3> templist = null;
                //Vector3 temp;
                this.GetComponent<FlexAnimation>().enabled = false;//might have to move this to start to optimize
                flyCam.enabled = true;
                playerController.enabled = false;
                //StartCoroutine(MoveParticle());
                //int x = this.GetComponent<FlexMouseDrag>().m_mouseParticle;


                if (doneMoving)
                {
                    turnOffAnim = false;
                    doneMoving = false;
                    flyCamEnable = false;
                    print(reset.position);
                    playerController.enabled = true;
                }
                
            }

            if (turnOnAnim)
            {
                
                //Note: need to save the behavior as an asset possibly or JSON?, need to ask stefan.
                //string jsonData = JSONSerializer.ToJson<Dictionary<int, Vector3>>(behavior);
                //File.WriteAllText(Application.persistentDataPath + "/test.json", jsonData);

                // test C# way of serializing:
                
                XmlSerializer xmlserializer = new XmlSerializer(labeledBehavior.GetType());
                FileStream file = File.Open(Application.persistentDataPath + "/labbehaviorTrial2.xml", FileMode.OpenOrCreate);
                xmlserializer.Serialize(file, labeledBehavior);
                file.Close();
                
                //JSONSerializer.Save<Dictionary<int, Vector3>>("test", behavior);
                if (behaviorName != null)
                {
                    print(behaviorName);
                }
                this.GetComponent<FlexAnimation>().enabled = true;
                turnOnAnim = false;
          
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


        void OnGUI()
        {
            if (!btnTexture)
            {
                //Debug.LogError("Please assign a texture on the inspector");
                return;
            }

            if (GUI.Button(new Rect(10, 10, 50, 50), "Move particle"))
            {
               // Debug.Log("Clicked the button with text");
                turnOffAnim = true;
            }

            if (GUI.Button(new Rect(10, 70, 50, 50), "Done Moving"))
            {
                //Debug.Log("Clicked the button with text");
                doneMoving = true;
            }

            if (GUI.Button(new Rect(10, 150, 50, 30), "Set  behavior"))
            {
                //Debug.Log("Clicked the button with text");
                turnOnAnim = true;
            }
        }

    }

    
