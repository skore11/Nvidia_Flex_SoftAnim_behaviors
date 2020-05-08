using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using uFlex;

//Define various behaviors as reactions to environment ex: scared, calm, preying 
//Have to acccess the Activate Voxels script to allow for each of the behaviors
//Activate voxels differently for a given scenario

public class GetBehaviors : MonoBehaviour {
    //storing the bounds behaviors first
    public Dictionary<int, Component> behaviorBounds = new Dictionary<int, Component>();
    //public int[] behaviorBoundsIndices = { 0, 1, 0, 1 };

    public SerializableMap<string, SerializableMap<int, Vector3>> localContainer;

    public bool gotXML;

    Dropdown m_Dropdown;
    //public Text m_Text;

    public GameObject flexObject;

    void Start()
    {
        //localContainer = FindObjectOfType<CreateBehavior>().container;
        gotXML = false;
        

        var comp = flexObject.GetComponents(typeof(IStorable));
        //Check how many behaviors there are on an Object:
        //Debug.Log(comp.Length);
        m_Dropdown = FindObjectOfType<Dropdown>();
        

        for (int i = 0; i < comp.Length; i++)
        {
            //print(i);
            //print(comp[i]);
            
            behaviorBounds.Add(i, comp[i]);
            //print(behaviorBounds[i].name);
        }

    }

     void Update()
    {
        if (gotXML)
        {
            flexObject.GetComponent<ContextBehavior>().localBehavior = localContainer;
            //this.GetComponent<Context>
            foreach (var index in localContainer)
            {
                print(index.Key);
                print(index.Value);
                m_Dropdown.options.Add(new Dropdown.OptionData(index.Key));
            }
            gotXML = false;
        }

    }
    //void DropdownValueChanged(Dropdown change)
    //{
    //    m_Text.text = "New Value : " + change.value;
    //}

    private void OnGUI()
    {

        GUILayout.BeginArea(new Rect((Screen.width) - 200, (Screen.height) - 600, 400, 50));
        foreach (var i in behaviorBounds)
        {
           
            //print(i.Value);
            int x = i.Key;
            //print(x);
            //Debug.Log(i.Value.ToString());
            Component temp = i.Value;


            if (GUILayout.Button(temp.ToString(), GUILayout.Width(200), GUILayout.Height(25))) 
            {
                (temp as MonoBehaviour).enabled = true;
            }
      
        }
        GUILayout.EndArea();
    }
}
