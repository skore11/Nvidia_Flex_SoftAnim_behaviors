using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uFlex
{
    public class AddFlexCollider : MonoBehaviour//FlexProcessor
    {
        Transform[] children; 
        // Use this for initialization
        void OnEnable()
        {
            Transform child = gameObject.transform.GetChild(0);
            SphereCollider sc = child.gameObject.AddComponent<SphereCollider>() as SphereCollider;
            sc.isTrigger = enabled;
            sc.radius = sc.radius * 10.0f;
            child.gameObject.AddComponent<TriggerParent>();
            //Debug.Log(child.name);

            //print(children[0].name);
            //Destroy(gameObject.GetComponent<SphereCollider>(), 5.0f);
            //Destroy(gameObject.GetComponent<TriggerParent>(), 5.0f);
            //Destroy(children[0].GetComponent<SphereCollider>(), 5.0f);
            //Destroy(children[0].GetComponent<TriggerParent>(), 5.0f);//remove the parent object sphere collider
        }

    }
}

