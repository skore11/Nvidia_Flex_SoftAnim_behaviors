using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackUnityObjMovement : MonoBehaviour
{
    // local copy of transform to be able to detect it being moved
    private float transformX;
    private float transformY;
    private float transformZ;


    void Start()
    {
        cacheTransform();
    }

    void cacheTransform() {
        transformX = this.transform.position.x;
        transformY = this.transform.position.y;
        transformZ = this.transform.position.z;
    }

    void Update() {
        if (UnityObjectHasMoved())
        {
            // copy over movement
            Debug.Log("detected movement");

            // and then update transform cache:
            cacheTransform();
        }
    }


    bool UnityObjectHasMoved() {
        return (transformX != this.transform.position.x
                || transformY != this.transform.position.y
                || transformZ != this.transform.position.z);
    }

}
