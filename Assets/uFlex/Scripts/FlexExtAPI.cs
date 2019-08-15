using System;
using System.Runtime.InteropServices;

using UnityEngine;

namespace uFlex
{
    /// <summary>
    /// A wrapper around (an open-source) flexExt.h. 
    /// Note that uFlex aims to recreate FlexExt funcionality in Unity.
    /// Here we use only efficient native methods for flex asset creation. 
    /// </summary>
    public class FlexExt
    {

        /// <summary>
        /// A structure compatible with native FlexExtAsset
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct FlexExtAsset
        {

            public IntPtr mParticles;

            public int mNumParticles;
            public int mMaxParticles;

            /// int*
            public IntPtr mSpringIndices;

            /// float*
            public IntPtr mSpringCoefficients;

            /// float*
            public IntPtr mSpringRestLengths;

            public int mNumSprings;

            /// int*
            public IntPtr mShapeIndices;
            public int mNumShapeIndices;

            /// int*
            public IntPtr mShapeOffsets;

            /// float*
            public IntPtr mShapeCoefficients;

            /// float*
            public IntPtr mShapeCenters;

            public int mNumShapes;
            public IntPtr mTriangleIndices;
            public int mNumTriangles;

            [MarshalAsAttribute(UnmanagedType.I1)]
            public bool mInflatable;

            public float mInflatableVolume;
            public float mInflatablePressure;
            public float mInflatableStiffness;
        }


        /// <summary>
        /// A structure compatible with native FlexExtAsset
        /// </summary>
        [Serializable]
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct FlexExtTearingClothAsset
        {

            public IntPtr mParticles;
            public int mNumParticles;
            public int mMaxParticles;

            /// int*
            public IntPtr mSpringIndices;

            /// float*
            public IntPtr mSpringCoefficients;

            /// float*
            public IntPtr mSpringRestLengths;

            public int mNumSprings;

            /// int*
            public IntPtr mShapeIndices;
            public int mNumShapeIndices;

            /// int*
            public IntPtr mShapeOffsets;

            /// float*
            public IntPtr mShapeCoefficients;

            /// float*
            public IntPtr mShapeCenters;

            public int mNumShapes;
            public IntPtr mTriangleIndices;
            public int mNumTriangles;

            [MarshalAsAttribute(UnmanagedType.I1)]
            public bool mInflatable;

            public float mInflatableVolume;
            public float mInflatablePressure;
            public float mInflatableStiffness;

            //only this is added
            public IntPtr mClothMesh;
        }



        /// <summary>
        /// Particles and vertices may need to be copied during tearing. 
        /// Because the user may maintain particle data outside of Flex, 
        /// this structure describes how to update the particle data. 
        /// The application should copy each existing particle given 
        /// by srcIndex (in the asset's particle array) to the destIndex (also in the asset's array).
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct FlexExtTearingParticleClone
        {
            public int srcIndex;
            public int destIndex;
        }

        /// <summary>
        /// The mesh topology may need to be updated to reference new particles during tearing. Because the user may maintain mesh topology outside of Flex, 
        /// this structure describes the necessary updates that should be performed on the mesh. 
        /// The triIndex member is the index of the index to be updated,
       ///  e.g.: a triIndex value of 4 refers to the index 1 vertex (4%3) of the index 1 triangle (4/3).
       ///  This entry in the indices array should be updated to point to the newParticleIndex.
        /// </summary>
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct FlexExtTearingMeshEdit
        {
            public int triIndex;
            public int newParticleIndex;
        }

        /// <summary>
        /// Create a shape body asset from a closed triangle mesh. The mesh is first voxelized at a spacing specified by the radius, and particles are placed at occupied voxels.
        /// </summary>
        /// <param name="vertices">Vertices of the triangle mesh</param>
        /// <param name="numVertices">The number of vertices</param>
        /// <param name="indices">The triangle indices</param>
        /// <param name="numTriangleIndices">	The number of triangles indices (triangles*3)</param>
        /// <param name="radius">The spacing used for voxelization, note that the number of voxels grows proportional to the inverse cube of radius, currently this method limits construction to resolutions smaller than 64^3</param>
        /// <param name="expand">Particles will be moved inwards (if negative) or outwards (if positive) from the surface of the mesh according to this factor</param>
        /// <returns>A pointer to an asset structure holding the particles and constraints</returns>
        /// 

#if (UNITY_32 || UNITY_EDITOR_32)
            [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateRigidFromMesh")]
#else
        [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateRigidFromMesh")]
        #endif
        public static extern IntPtr flexExtCreateRigidFromMesh(Vector3[] vertices, int numVertices, int[] indices, int numTriangleIndices, float radius, float expand);


        /// <summary>
        /// Create a shape body asset from a closed triangle mesh. The mesh is first voxelized at a spacing specified by the radius, and particles are placed at occupied voxels.
        /// </summary>
        /// <param name="vertices">Vertices of the triangle mesh</param>
        /// <param name="numVertices">	The number of vertices</param>
        /// <param name="indices">The triangle indices</param>
        /// <param name="numTriangleIndices">The number of triangles indices (triangles*3)</param>
        /// <param name="particleSpacing">	The spacing to use when creating particles</param>
        /// <param name="volumeSampling">Control the resolution the mesh is voxelized at in order to generate interior sampling, if the mesh is not closed then this should be set to zero and surface sampling should be used instead</param>
        /// <param name="surfaceSampling">Controls how many samples are taken of the mesh surface, this is useful to ensure fine features of the mesh are represented by particles, or if the mesh is not closed</param>
        /// <param name="clusterSpacing">The spacing for shape-matching clusters, should be at least the particle spacing</param>
        /// <param name="clusterRadius">Controls the overall size of the clusters, this controls how much overlap the clusters have which affects how smooth the final deformation is, if parts of the body are detaching then it means the clusters are not overlapping sufficiently to form a fully connected set of clusters</param>
        /// <param name="clusterStiffness">Controls the stiffness of the resulting clusters</param>
        /// <param name="linkRadius">	Any particles below this distance will have additional distance constraints created between them</param>
        /// <param name="linkStiffness">	The stiffness of distance links</param>
        /// <returns>A pointer to an asset structure holding the particles and constraints</returns>

        #if (UNITY_32 || UNITY_EDITOR_32)
                [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateSoftFromMesh")]
        #else
                [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateSoftFromMesh")]
        #endif
        public static extern IntPtr flexExtCreateSoftFromMesh(Vector3[] vertices, int numVertices, int[] indices, int numTriangleIndices, float particleSpacing, float volumeSampling, float surfaceSampling, float clusterSpacing, float clusterRadius, float clusterStiffness, float linkRadius, float linkStiffness);



        /// <summary>
        /// Create an index buffer of unique vertices in the mesh
        /// </summary>
        /// <param name="vertices">A pointer to an array of float3 positions</param>
        /// <param name="numVertices">The number of vertices in the mesh</param>
        /// <param name="uniqueVerts">A list of unique mesh vertex indices, should be numVertices in length (worst case all verts are unique)</param>
        /// <param name="originalToUniqueMap">Mapping from the original vertex index to the unique vertex index, should be numVertices in length</param>
        /// <param name="threshold">The distance below which two vertices are considered duplicates</param>
        /// <returns>The number of unique vertices in the mesh</returns>
        #if (UNITY_32 || UNITY_EDITOR_32)
                [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateWeldedMeshIndices")]
        #else
                [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateWeldedMeshIndices")]
        #endif
        //    public static extern int flexExtCreateWeldedMeshIndices(Vector3[] vertices, int numVertices, out int[] uniqueVerts, out int[] originalToUniqueMap, float threshold);
        public static extern int flexExtCreateWeldedMeshIndices(Vector3[] vertices, int numVertices, int[] uniqueVerts, int[] originalToUniqueMap, float threshold);


        /// <summary>
        /// Create a cloth asset consisting of stretch and bend distance constraints given an indexed triangle mesh. Stretch constraints will be placed along triangle edges, while bending constraints are placed over two edges.
        /// </summary>
        /// <param name="particles">Positions and masses of the particles in the format [x, y, z, 1/m]</param>
        /// <param name="numParticles">The number of particles</param>
        /// <param name="indices">The triangle indices, these should be 'welded' using flexExtCreateWeldedMeshIndices() first</param>
        /// <param name="numTriangles">The number of triangles</param>
        /// <param name="stretchStiffness">The stiffness coefficient for stretch constraints</param>
        /// <param name="bendStiffness">The stiffness coefficient used for bending constraints</param>
        /// <param name="tetherStiffness">	If > 0.0f then the function will create tethers attached to particles with zero inverse mass. These are unilateral, long-range attachments, which can greatly reduce stretching even at low iteration counts.</param>
        /// <param name="tetherGive">Because tether constraints are so effective at reducing stiffness, it can be useful to allow a small amount of extension before the constraint activates.</param>
        /// <param name="pressure">	If > 0.0f then a volume (pressure) constraint will also be added to the asset, the rest volume and stiffness will be automatically computed by this function</param>
        /// <returns>A pointer to an asset structure holding the particles and constraints</returns>
        #if (UNITY_32 || UNITY_EDITOR_32)
                        [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateClothFromMesh")]
        #else
        [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateClothFromMesh")]
        #endif
        public static extern IntPtr flexExtCreateClothFromMesh(Vector4[] particles, int numParticles, int[] indices, int numTriangles, float stretchStiffness, float bendStiffness, float tetherStiffness, float tetherGive, float pressure);


        /// <summary>
        /// Creates information for linear blend skining a graphics mesh to a set of transforms (bones)
        /// </summary>
        /// <param name="vertices">Vertices of the triangle mesh</param>
        /// <param name="numVertices">The number of vertices</param>
        /// <param name="bones">Pointer to an array of vec3 positions representing the bone positions</param>
        /// <param name="numBones">The number of bones</param>
        /// <param name="falloff">The speed at which the bone's influence on a vertex falls off with distance</param>
        /// <param name="maxDistance">The maximum distance a bone can be from a vertex before it will not influence it any more</param>
        /// <param name="skinningWeights">[OUT] The normalized weights for each bone, there are up to 4 weights per-vertex so this should be numVertices*4 in length</param>
        /// <param name="skinningIndices">[OUT] The indices of each bone corresponding to the skinning weight, will be -1 if this weight is not used</param>
#if (UNITY_32 || UNITY_EDITOR_32)
                [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateSoftMeshSkinning")]
#else
        [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateSoftMeshSkinning")]
        #endif
        public static extern void flexExtCreateSoftMeshSkinning(Vector3[] vertices, int numVertices, Vector3[] bones, int numBones, float falloff, float maxDistance,  float[] skinningWeights, int[] skinningIndices);


        /// <summary>
        /// Create a cloth asset consisting of stretch and bend distance constraints given an indexed triangle mesh. 
        /// This creates an asset with the same structure as flexExtCreateClothFromMesh(), 
        /// however tether constraints are not supported, and additional information regarding mesh topology will be stored with the asset.
        /// </summary>
        /// <param name="particles">Positions and masses of the particles in the format [x, y, z, 1/m]</param>
        /// <param name="numParticles">The number of particles</param>
        /// <param name="indices">The triangle indices, these should be 'welded' using flexExtCreateWeldedMeshIndices() first</param>
        /// <param name="numTriangles">The number of triangles</param>
        /// <param name="stretchStiffness">The stiffness coefficient for stretch constraints</param>
        /// <param name="bendStiffness">The stiffness coefficient used for bending constraints</param>
        /// <param name="tetherStiffness">	If > 0.0f then the function will create tethers attached to particles with zero inverse mass. These are unilateral, long-range attachments, which can greatly reduce stretching even at low iteration counts.</param>
        /// <param name="tetherGive">Because tether constraints are so effective at reducing stiffness, it can be useful to allow a small amount of extension before the constraint activates.</param>
        /// <param name="pressure">	If > 0.0f then a volume (pressure) constraint will also be added to the asset, the rest volume and stiffness will be automatically computed by this function</param>
        /// <returns>A pointer to an asset structure holding the particles and constraints</returns>
        #if (UNITY_32 || UNITY_EDITOR_32)
            [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateTearingClothFromMesh")]
        #else
            [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateTearingClothFromMesh")]
        #endif
        public static extern IntPtr flexExtCreateTearingClothFromMesh(Vector4[] particles, int numParticles, int maxParticles, int[] indices, int numTriangles, float stretchStiffness, float bendStiffness, float pressure);

        /// <summary>
        /// Perform cloth mesh tearing, this function will calculate the strain on each distance constraint and perform splits if it is above a certain strain threshold (i.e.: length/restLength > maxStrain).
        /// </summary>
        /// <param name="asset">The asset describing the cloth constraint network, this must be created with flexExtCreateTearingClothFromMesh()</param>
        /// <param name="maxStrain">The maximum allowable strain on each edge</param>
        /// <param name="maxSplits">The maximum number of constraint breaks that will be performed, this controls the 'rate' of mesh tearing</param>
        /// <param name="particleCopies">Pointer to an array of FlexExtTearingParticleClone structures that describe the particle copies that need to be performed</param>
        /// <param name="numParticleCopies">Pointer to an integer that will have the number of copies performed written to it</param>
        /// <param name="maxCopies">The maximum number of particle copies that will be performed, multiple particles copies may be performed in response to one split</param>
        /// <param name="triangleEdits">Pointer to an array of FlexExtTearingMeshEdit structures that describe the topology updates that need to be performed</param>
        /// <param name="numTriangleEdits">Pointer to an integer that will have the number of topology updates written to it</param>
        /// <param name="maxEdits">The maximum number of index buffer edits that will be output</param>
            #if (UNITY_32 || UNITY_EDITOR_32)
                            [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtTearClothMesh")]
            #else
                        [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtTearClothMesh")]
            #endif
            public static extern void flexExtTearClothMesh(ref FlexExtAsset asset, float maxStrain, int maxSplits, ref FlexExtTearingParticleClone particleCopies, ref int numParticleCopies, int maxCopies, ref FlexExtTearingMeshEdit triangleEdits, ref int numTriangleEdits, int maxEdits);
       


        /// <summary>
        /// Destroy an asset created with flexExtCreateTearingClothFromMesh()
        /// </summary>
        /// <param name="flexAsset"></param>
        #if (UNITY_32 || UNITY_EDITOR_32)
            [DllImportAttribute("flexExtRelease_x86", EntryPoint = "flexExtCreateClothFromMesh")]
        #else
            [DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateClothFromMesh")]
        #endif
        public static extern void flexExtDestroyTearingCloth(IntPtr flexAsset);




        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexExtContainer
        //{
        //}

        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexTimers
        //{
        //}

        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexSolver
        //{
        //}

        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexMemory
        //{
        //}

        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexExtInstance
        //{

        //    /// int*
        //    public IntPtr mParticleIndices;

        //    /// int
        //    public int mNumParticles;

        //    /// int
        //    public int mTriangleIndex;

        //    /// int
        //    public int mShapeIndex;

        //    /// int
        //    public int mInflatableIndex;

        //    /// float*
        //    public IntPtr mShapeTranslations;

        //    /// float*
        //    public IntPtr mShapeRotations;

        //    /// FlexExtAsset*
        //    public IntPtr mAsset;

        //    /// void*
        //    public IntPtr mUserData;
        //}

        //public enum FlexForceExtMode
        //{

        //    /// eFlexExtModeForce -> 0
        //    eFlexExtModeForce = 0,

        //    /// eFlexExtModeImpulse -> 1
        //    eFlexExtModeImpulse = 1,

        //    /// eFlexExtModeVelocityChange -> 2
        //    eFlexExtModeVelocityChange = 2,
        //}

        //[StructLayoutAttribute(LayoutKind.Sequential)]
        //public struct FlexExtForceField
        //{

        //    /// float[3]
        //    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.R4)]
        //    public float[] mPosition;

        //    /// float
        //    public float mRadius;

        //    /// float
        //    public float mStrength;

        //    /// FlexForceExtMode
        //    public FlexForceExtMode mMode;

        //    /// boolean
        //    [MarshalAsAttribute(UnmanagedType.I1)]
        //    public bool mLinearFalloff;
        //}








        ///// Return Type: FlexExtAsset*
        /////particles: float*
        /////numParticles: int
        /////maxParticles: int
        /////indices: int*
        /////numTriangles: int
        /////stretchStiffness: float
        /////bendStiffness: float
        /////pressure: float
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateTearingClothFromMesh")]
        //public static extern IntPtr flexExtCreateTearingClothFromMesh(ref float particles, int numParticles, int maxParticles, ref int indices, int numTriangles, float stretchStiffness, float bendStiffness, float pressure);




        ///// Return Type: void
        /////asset: FlexExtAsset*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtDestroyAsset")]
        //public static extern void flexExtDestroyAsset(ref FlexExtAsset asset);




        ///// Return Type: FlexExtContainer*
        /////solver: FlexSolver*
        /////maxParticles: int
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateContainer")]
        //public static extern IntPtr flexExtCreateContainer(ref FlexSolver solver, int maxParticles);


        ///// Return Type: void
        /////container: FlexExtContainer*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtDestroyContainer")]
        //public static extern void flexExtDestroyContainer(ref FlexExtContainer container);


        ///// Return Type: int
        /////container: FlexExtContainer*
        /////n: int
        /////indices: int*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtAllocParticles")]
        //public static extern int flexExtAllocParticles(ref FlexExtContainer container, int n, ref int indices);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////n: int
        /////indices: int*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtFreeParticles")]
        //public static extern void flexExtFreeParticles(ref FlexExtContainer container, int n, ref int indices);


        ///// Return Type: int
        /////container: FlexExtContainer*
        /////indices: int*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtGetActiveList")]
        //public static extern int flexExtGetActiveList(ref FlexExtContainer container, ref int indices);


        ///// Return Type: FlexExtInstance*
        /////container: FlexExtContainer*
        /////asset: FlexExtAsset*
        /////transform: float*
        /////vx: float
        /////vy: float
        /////vz: float
        /////phase: int
        /////invMassScale: float
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtCreateInstance")]
        //public static extern IntPtr flexExtCreateInstance(ref FlexExtContainer container, ref FlexExtAsset asset, ref float transform, float vx, float vy, float vz, int phase, float invMassScale);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////instance: FlexExtInstance*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtDestroyInstance")]
        //public static extern void flexExtDestroyInstance(ref FlexExtContainer container, ref FlexExtInstance instance);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////asset: FlexExtAsset*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtNotifyAssetChanged")]
        //public static extern void flexExtNotifyAssetChanged(ref FlexExtContainer container, ref FlexExtAsset asset);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////particles: float**
        /////restParticles: float**
        /////velocities: float**
        /////phases: int**
        /////normals: float**
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtGetParticleData")]
        //public static extern void flexExtGetParticleData(ref FlexExtContainer container, ref IntPtr particles, ref IntPtr restParticles, ref IntPtr velocities, ref IntPtr phases, ref IntPtr normals);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////indices: int**
        /////normals: float**
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtGetTriangleData")]
        //public static extern void flexExtGetTriangleData(ref FlexExtContainer container, ref IntPtr indices, ref IntPtr normals);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////rotations: float**
        /////positions: float**
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtGetShapeData")]
        //public static extern void flexExtGetShapeData(ref FlexExtContainer container, ref IntPtr rotations, ref IntPtr positions);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////lower: float*
        /////upper: float*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtGetBoundsData")]
        //public static extern void flexExtGetBoundsData(ref FlexExtContainer container, ref float lower, ref float upper);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////dt: float
        /////numSubsteps: int
        /////timers: FlexTimers*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtTickContainer")]
        //public static extern void flexExtTickContainer(ref FlexExtContainer container, float dt, int numSubsteps, ref FlexTimers timers);


        ///// Return Type: void
        /////container: FlexExtContainer*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtPushToDevice")]
        //public static extern void flexExtPushToDevice(ref FlexExtContainer container);


        ///// Return Type: void
        /////container: FlexExtContainer*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtPullFromDevice")]
        //public static extern void flexExtPullFromDevice(ref FlexExtContainer container);


        ///// Return Type: void
        /////container: FlexExtContainer*
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtUpdateInstances")]
        //public static extern void flexExtUpdateInstances(ref FlexExtContainer container);


        ///// Return Type: void
        /////container: FlexExtContainer*
        /////forceFields: FlexExtForceField*
        /////numForceFields: int
        /////source: FlexMemory
        //[DllImportAttribute("flexExtRelease_x64", EntryPoint = "flexExtSetForceFields")]
        //public static extern void flexExtSetForceFields(ref FlexExtContainer container, ref FlexExtForceField forceFields, int numForceFields, FlexMemory source);



    }
}
