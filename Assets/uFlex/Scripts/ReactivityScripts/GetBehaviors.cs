using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Define various behaviors as reactions to environment ex: scared, calm, preying 
//Have to acccess the Activate Voxels script to allow for each of the behaviors
//Activate voxels differently for a given scenario

public class GetBehaviors : MonoBehaviour {
    //storing the bounds behaviors first
    public Dictionary<int, Component> behaviorBounds = new Dictionary<int, Component>();
    //public int[] behaviorBoundsIndices = { 0, 1, 0, 1 };

    void Start()
    {
        var comp = GetComponents(typeof(IStorable));
        //Check how many behaviors there are on an Object:
        //Debug.Log(comp.Length);
        for (int i = 0; i < comp.Length; i++)
        {
            //print(i);
            //print(comp[i]);
            
            behaviorBounds.Add(i, comp[i]);
            //print(behaviorBounds[i].name);
        }

    }

    //TODO: simple GUI for selecting behaviors, trigger behavior in associated script on GUI button 
    //might have to use switch case

    private void OnGUI()
    {
        foreach (var i in behaviorBounds)
        {
           
            //print(i.Value);
            int x = i.Key;
            //print(x);
            //Debug.Log(i.Value.ToString());
            Component temp = i.Value;
            if (GUILayout.Button(i.Value.ToString()))
            {
                (temp as MonoBehaviour).enabled = true;
            }
      
        }
    }
}
