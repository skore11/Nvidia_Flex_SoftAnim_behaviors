using System.Collections;
using System.Collections.Generic;
using uFlex;
using UnityEngine;


public class TrackUnityObjMovement : FlexProcessor {
    // this needs to be a FlexProcessor, otherwise we have no guarantee that the movement will not be copied to the
    // particles positions in between a flex run, and then it would just be overwritten.

    // local copy of transform to be able to detect it being moved
    private float transformX;
    private float transformY;
    private float transformZ;
    private Matrix4x4 transforms;
    // local reference to the FlexParticles
    private FlexParticles fParticles;


    public void Awake() {
        this.fParticles = GetComponent<FlexParticles>();
    }

    public void Start() {
        //CacheTransform();
        CacheLocalTransform();
    }

    //private void CacheTransform() {
    //    transformX = this.transform.position.x;
    //    transformY = this.transform.position.y;
    //    transformZ = this.transform.position.z;
    //}

    private void CacheLocalTransform()
    {
        transforms = this.transform.localToWorldMatrix;
    }

    public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters) {
        //if (UnityObjectHasMoved())
        ///* if (UnityObjectTransformchanged())*/
        //{
        //    // copy over movement
        //    //Debug.Log("detected movement");
        //    Vector3 movement = this.transform.position - new Vector3(transformX, transformY, transformZ);
            
        //    TranslateFlexPositions(movement);
        //    // and then update transform cache:
        //    CacheTransform();
        //}

        if (transform.hasChanged)
        {
            TransformFlexPositions(transforms);
            CacheLocalTransform();
        }
    }

    //private bool UnityObjectHasMoved() /*private bool UnityObjectTransformchanged()*/
    //{
    //    return (transformX != this.transform.position.x
    //            || transformY != this.transform.position.y
    //            || transformZ != this.transform.position.z
    //            );
    //    //return (transforms != this.transform.localToWorldMatrix);
    //}

    public void TranslateFlexPositions(Vector3 movement) {
        // update the particles since they are in world coordinates (!!!) after initialization, no need to update the restparticles
        var particles = fParticles.m_particles;
        for (int pId = 0; pId < fParticles.m_particlesCount; pId++)
        {
            particles[pId].pos += movement;
        }
        // actually no need to reset the initialized flag. flex will copy the new values to the GPU on next container update,
        // as long as this update happens before that and not in between (then it would be overwritten by values coming back from the GPU)
        // we make sure of that by making this update inside precontainerupdate.
    }

    public void TransformFlexPositions(Matrix4x4 temp)
    {
        Matrix4x4 inverse = this.transform.localToWorldMatrix.inverse;
        Matrix4x4 newTransform = this.transform.localToWorldMatrix;
        Matrix4x4 inverseOld = temp.inverse;
        
        // update the particles since they are in world coordinates (!!!) after initialization, no need to update the restparticles
        var particles = fParticles.m_particles;
        for (int pId = 0; pId < fParticles.m_particlesCount; pId++)
        {
            particles[pId].pos = inverseOld.MultiplyPoint3x4(particles[pId].pos);
            particles[pId].pos = newTransform.MultiplyPoint3x4(particles[pId].pos);
        }
        // actually no need to reset the initialized flag. flex will copy the new values to the GPU on next container update,
        // as long as this update happens before that and not in between (then it would be overwritten by values coming back from the GPU)
        // we make sure of that by making this update inside precontainerupdate.
    }


}
