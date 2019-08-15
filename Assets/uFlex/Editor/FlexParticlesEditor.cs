using UnityEngine;
using UnityEditor;
using System.Collections;
namespace uFlex
{
    [CustomEditor(typeof(FlexParticles))]
    public class FlexParticlesEditor : Editor
    {

        void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        }

        public void OnSceneGUI()
        {

        }
    }
}
