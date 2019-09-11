using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AddFlexCollider : MonoBehaviour
{




    // Use this for initialization
    void OnEnable()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            SphereCollider sc = child.gameObject.AddComponent<SphereCollider>() as SphereCollider;
            sc.radius = sc.radius * 20.0f;
            //Debug.Log(child.name);
        }

        Destroy(children[0].GetComponent<SphereCollider>());//remove the parent object
    }




    //#if UNITY_EDITOR
    //    public void AddCollider()
    //    {
    //        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
    //        foreach (Transform child in children)
    //        {
    //            SphereCollider sc = child.gameObject.AddComponent<SphereCollider>() as SphereCollider;
    //            Debug.Log(child.name);
    //        }
    //    }
    //#endif


}

