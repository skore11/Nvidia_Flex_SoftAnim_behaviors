using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//Define various behaviors as reactions to environment ex: scared, calm, preying 
//Have to acccess the Activate Voxels script to allow for each of the behaviors
//Activate voxels differently for a given scenario

public class GetBehaviors : MonoBehaviour {
    //storing the bounds behaviors first
    public Dictionary<int, Component> behaviorBounds = new Dictionary<int, Component>();
    //public int[] behaviorBoundsIndices = { 0, 1, 0, 1 };

    private SerializableMap<string, SerializableMap<int, Vector3>> localContainer;

    Dropdown m_Dropdown;
    public Text m_Text;

    void Start()
    {
        localContainer = FindObjectOfType<CreateBehavior>().container;

        var comp = GetComponents(typeof(IStorable));
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

        foreach (var i in behaviorBounds)
        {

            //print(i.Value);
            int x = i.Key;
            //print(x);
            //Debug.Log(i.Value.ToString());
            Component temp = i.Value;

            m_Dropdown.options[x].text = temp.ToString();

          
        }

        


    }


    void DropdownValueChanged(Dropdown change)
    {
        m_Text.text = "New Value : " + change.value;
    }

    private void OnGUI()
    {
        foreach (var i in behaviorBounds)
        {
           
            //print(i.Value);
            int x = i.Key;
            //print(x);
            //Debug.Log(i.Value.ToString());
            Component temp = i.Value;
            

            if (GUILayout.Button(temp.ToString()))
            {
                (temp as MonoBehaviour).enabled = true;
            }
      
        }
    }
}
