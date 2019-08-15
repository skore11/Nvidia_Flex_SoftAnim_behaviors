using System;
using System.Runtime.InteropServices;

using UnityEngine;

namespace uFlex
{

    public class uFlexAPI
    {


        [DllImportAttribute("uFlex", EntryPoint = "uFlexCreateTearableClothMesh")]
        public static extern IntPtr uFlexCreateTearableClothMesh(Particle[] particles, int numParticles, int maxParticles, int[] indices, int numTriangles, float stretchStiffness, float bendStiffness, float pressure);

        [DllImportAttribute("uFlex", EntryPoint = "uFlexCreateTearableClothMesh")]
        public static extern IntPtr uFlexCreateTearableClothMesh(Vector4[] particles, int numParticles, int maxParticles, int[] indices, int numTriangles, float stretchStiffness, float bendStiffness, float pressure);

        [DllImportAttribute("uFlex", EntryPoint = "uFlexTearClothMesh")]
        public static extern void uFlexTearClothMesh(ref int mNumParticles, int mMaxParticles, int mNumSprings, Particle[] mParticles, int[] mSpringIndices, float[] mSpringRestLengths, float[] mSpringStiffness, int[] mTriangleIndices,
		    IntPtr clothMesh, float maxStrain, int maxSplits, FlexExt.FlexExtTearingParticleClone[] particleCopies, ref int numParticleCopies, int maxCopies, FlexExt.FlexExtTearingMeshEdit[] triangleEdits, ref int numTriangleEdits, int maxEdits);
	
    }
}
