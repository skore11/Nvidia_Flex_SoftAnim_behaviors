using UnityEngine;
using System.Collections;

namespace uFlex
{
    /// <summary>
    /// Derive from this class to make modifications to the particles between the simulation steps.
    /// The Pre.PostContainerUpdate is called by the solver inbetween data pulls/pushes to the GPU.
    /// </summary>
    public class FlexProcessor : MonoBehaviour
    {

        // Overrideto modify the data just after the solver was created
        public virtual void FlexStart(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }

        // Override to modify the data after the flex game objects were updated but before the data from them was copied to the container arrays
        // In other words, use this for working with flex game objects.
        public virtual void PreContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }


        // Override to modify the data after the container was updated
        // In other words, use for working directly with the container data.
        public virtual void PostContainerUpdate(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }

        // Override this method to modify the data just before the solver is closed
        public virtual void FlexClose(FlexSolver solver, FlexContainer cntr, FlexParameters parameters)
        {

        }
    }
}