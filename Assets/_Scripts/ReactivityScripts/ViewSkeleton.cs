using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===========================================================================================
// Summary
//===========================================================================================

//Function: this script is used to view the character's skeleton in DrawGizmos() 
//Skeleton scales with skinned mesh renderer of character model.
//Can be used to debug bone positions
//Should be used to transfer bone transforms to nearest mass neighbor in world coordinates
//Ultimately the skeleton will be used to drive the active ragdolls motions

public class ViewSkeleton : MonoBehaviour {

    public Transform rootNode;
    public Transform[] childNodes;//33 bones for octopus
   
    void OnDrawGizmosSelected()
    {
        if (rootNode != null)
        {
            if (childNodes == null|| childNodes.Length == 0)
            {
                //get all joints to draw
                PopulateChildren();
            }


            foreach (Transform child in childNodes)
            {

                if (child == rootNode)
                {
        //list includes the root, green cube for rootnode
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(child.position, new Vector3(.1f, .1f, .1f));

                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(child.position, child.parent.position);
                    //float dist = Vector3.Distance(child.position, child.parent.position);
                    float linepos = 16.0f;
            //For each of distance between child and parent bone, segment line into portions and add a red cude at every segment
                    for (int s =1; s<=16; s++)
                    {
                        Debug.Log(s);
                        float percentage = s / linepos;
                        Debug.Log(percentage);
                        Vector3 split = Vector3.Lerp(child.position, child.parent.position, percentage);
                        
                        
                        Gizmos.DrawCube(split, new Vector3(0.01f, 0.01f, 0.01f));
                    }
                }
            }

        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}
