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
    // local reference to the FlexParticles
    private FlexParticles fParticles;


    public void Awake() {
        this.fParticles = GetComponent<FlexParticles>();
    }

    public void Start() {
        CacheTransform();
    }

    private void CacheTransform() {
        transformX = this.transform.position.x;
        transformY = this.transform.position.y;
        transformZ = this.transform.position.z;
    }

    public override void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters) {
        if (UnityObjectHasMoved()) {
            // copy over movement
            //Debug.Log("detected movement");
            Vector3 movement = this.transform.position - new Vector3(transformX, transformY, transformZ);
            TranslateFlexPositions(movement);
            // and then update transform cache:
            CacheTransform();
        }
    }

    private bool UnityObjectHasMoved() {
        return (transformX != this.transform.position.x
                || transformY != this.transform.position.y
                || transformZ != this.transform.position.z);
    }

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

}
