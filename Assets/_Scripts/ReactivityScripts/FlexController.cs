using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeSharpPlus;

public class FlexController : MonoBehaviour
{

    private UIController myUIcontroller;

    void Awake()
    {
        this.Initialize();
    }

    protected void Initialize()
    {
        this.myUIcontroller = this.GetComponent<UIController>();
    }

    protected void StartTree(
        Node root,
        BehaviorObject.StatusChangedEventHandler statusChanged = null)
    {
    }

    //#region Flex Helper Nodes

    //#region Iterations
    /// <summary>
    /// Changes the iteration count 
    /// </summary>
    //public Node Node_IterationChange(Val<float> iter)
    //{
    //    return new LeafInvoke
    //        (
    //        () => this.myUIcontroller.numOfIterations);
            
    //}
    // Update is called once per frame
    void Update()
    {
        
    }
}
