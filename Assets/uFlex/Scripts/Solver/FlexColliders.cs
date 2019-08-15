using System;
using UnityEngine;

namespace uFlex
{
    /// <summary>
    /// Holds all the data about the Flex colliders in the scene.
    /// Currently only Triangle Meshes are supported
    /// </summary>
    public class FlexColliders : MonoBehaviour
    {
        public MeshCollider[] m_meshColliders;
        public int m_collidersCount;

        public Flex.CollisionTriangleMesh[] m_collidersGeometry;

        public Vector4[] m_collidersPositions;
        public Vector4[] m_collidersPrevPositions;

        public Quaternion[] m_collidersRotations;
        public Quaternion[] m_collidersPrevRotations;

        public Vector4[] m_collidersAabbMin;
        public Vector4[] m_collidersAabbMax;

        public int[] m_collidersStarts;
        public int[] m_collidersFlags;


        public void ProcessColliders(IntPtr solverPtr, Flex.Memory memory)
        {
            m_meshColliders = FindObjectsOfType<MeshCollider>();
            m_collidersCount = m_meshColliders.Length;

            m_collidersGeometry = new Flex.CollisionTriangleMesh[m_collidersCount];

            m_collidersPositions = new Vector4[m_collidersCount];
            m_collidersPrevPositions = new Vector4[m_collidersCount];

            m_collidersRotations = new Quaternion[m_collidersCount];
            m_collidersPrevRotations = new Quaternion[m_collidersCount];

            m_collidersAabbMin = new Vector4[m_collidersCount];
            m_collidersAabbMax = new Vector4[m_collidersCount];

            m_collidersStarts = new int[m_collidersCount];
            m_collidersFlags = new int[m_collidersCount];

            for (int i = 0; i < m_collidersCount; i++)
            {
                Mesh mesh = m_meshColliders[i].GetComponent<MeshFilter>().mesh;
                MeshCollider meshCollider = m_meshColliders[i];
                Transform tr = m_meshColliders[i].transform;

                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;

                Vector3 localLowerBound = mesh.bounds.min;
                Vector3 localUpperBound = mesh.bounds.max;
                // FlexUtils.GetBounds(vertices, out localLowerBound, out localUpperBound);

                IntPtr meshPtr = Flex.CreateTriangleMesh();
                Flex.UpdateTriangleMesh(meshPtr, vertices, triangles, mesh.vertexCount, mesh.triangles.Length / 3, ref localLowerBound, ref localUpperBound, memory);

                //TODO
                //FlexAPI.FlexCollisionGeometry geo = new FlexAPI.FlexCollisionGeometry();
                //geo.mTriMesh.mMesh = meshPtr;
                //geo.mTriMesh.mScale = tr.lossyScale.x;

                //FlexAPI.FlexCollisionGeometry[] shapeGeometry = new FlexAPI.FlexCollisionGeometry[] { geo };

                Flex.CollisionTriangleMesh triCol = new Flex.CollisionTriangleMesh();
                triCol.mMesh = meshPtr;
                triCol.mScale = tr.lossyScale.x;

                m_collidersGeometry[i] = triCol;

                m_collidersPositions[i] = tr.position;
                m_collidersPrevPositions[i] = tr.position;

                m_collidersRotations[i] = tr.rotation;
                m_collidersPrevRotations[i] = tr.rotation;

                m_collidersAabbMin[i] = meshCollider.bounds.min;
                m_collidersAabbMax[i] = meshCollider.bounds.max;

                m_collidersStarts[i] = i;
                m_collidersFlags[i] = Flex.MakeShapeFlags(Flex.CollisionShapeType.eFlexShapeTriangleMesh, !m_meshColliders[i].gameObject.isStatic);
            }

            UpdateColliders(solverPtr, memory);
        }

        public void UpdateColliders(IntPtr solverPtr, Flex.Memory memory)
        {
            for (int i = 0; i < m_collidersCount; i++)
            {

              //  Mesh mesh = m_meshColliders[i].GetComponent<MeshFilter>().mesh;
                MeshCollider meshCollider = m_meshColliders[i];
                Transform tr = m_meshColliders[i].transform;

                m_collidersPrevPositions[i] = m_collidersPositions[i];
                m_collidersPositions[i] = tr.position;

                m_collidersPrevRotations[i] = m_collidersRotations[i];
                m_collidersRotations[i] = tr.rotation;

                m_collidersAabbMin[i] = meshCollider.bounds.min;
                m_collidersAabbMax[i] = meshCollider.bounds.max;

            }

            Flex.SetShapes(solverPtr, m_collidersGeometry, m_collidersGeometry.Length, m_collidersAabbMin, m_collidersAabbMax, m_collidersStarts, m_collidersPositions, m_collidersRotations,
            m_collidersPrevPositions, m_collidersPrevRotations, m_collidersFlags, m_collidersStarts.Length, memory);

        }

    }
}