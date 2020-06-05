using System.Collections;
using System.Collections.Generic;
using uFlex;
using UnityEngine;

public class MoveCollider : MyFlexParticlesLock
{
    //private BoxCollider myCol;

    
    // Start is called before the first frame update
    void Start()
    {
        //myCol = GetComponent<BoxCollider>();

    }

    // Update is called once per frame
    public override void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
    {

        moveCol = false;
        if (Input.GetKeyDown("o"))
        {
            //myCol.center += new Vector3(0.0f, 2.0f, 0.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
        if (Input.GetKeyDown("k"))
        {
            //myCol.center += new Vector3(0.0f, -2.0f, 0.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
        if (Input.GetKeyDown("j"))
        {
            //myCol.center += new Vector3(2.0f, 0.0f, 0.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
        if (Input.GetKeyDown("l"))
        {
            //myCol.center += new Vector3(-2.0f, 0.0f, 0.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
        if (Input.GetKeyDown("i"))
        {
            //myCol.center += new Vector3(0.0f, 0.0f, 2.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
        if (Input.GetKeyDown("p"))
        {
            //myCol.center += new Vector3(0.0f, 0.0f, -2.0f);
            moveCol = true;
            base.PostContainerUpdate(solver, cntr, parameters);
        }
    }
}
