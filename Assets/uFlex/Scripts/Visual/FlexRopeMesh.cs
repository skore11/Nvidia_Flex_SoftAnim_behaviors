using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uFlex
{
    /// <summary>
    /// Renders rope mesh using Hermite interpolation. Converted from FlexDemo
    /// </summary>
    public class FlexRopeMesh : MonoBehaviour
    {
        public FlexParticles m_particles;

        public float m_radius = 0.5f;
        public int m_resolution = 8;
        public int m_smoothing = 3;


        public Mesh m_mesh;


        [SerializeField, HideInInspector]
        private Vector3[] m_vertices;

        [SerializeField, HideInInspector]
        private Vector3[] m_normals;

        [SerializeField, HideInInspector]
        private int[] m_triangles;

 
        // Use this for initialization
        void Start()
        {
            m_particles = GetComponent<FlexParticles>();
        }

        // Update is called once per frame
        void Update()
        {

            DrawRope(transform, m_particles.m_particles, m_particles.m_particlesCount, ref m_vertices, ref m_normals, m_radius, m_resolution, m_smoothing);

            m_mesh.vertices = m_vertices;
            m_mesh.normals = m_normals;

            m_mesh.RecalculateBounds();

        }

        public void InitRope(int numPoints)
        {
            m_mesh = new Mesh();
            m_mesh.name = "RopeMesh";

            m_vertices = new Vector3[m_resolution * numPoints * m_smoothing];
            m_normals = new Vector3[m_resolution * numPoints * m_smoothing];
            m_triangles = new int[numPoints * m_resolution * 6 * m_smoothing];
            
            DrawRope(transform, m_particles.m_particles, numPoints, ref m_vertices, ref m_normals, m_radius, m_resolution, m_smoothing);

            UpdateRopeTriangles(numPoints, ref m_triangles, m_radius, m_resolution, m_smoothing);

            m_mesh.vertices = m_vertices;
            m_mesh.normals = m_normals;
            m_mesh.triangles = m_triangles;

            m_mesh.RecalculateBounds();
        }

        public void UpdateRopeTriangles(int numPoints, ref int[] trianglesOut, float radius, int resolution, int smoothing)
        {
            int startIndex = 0;
           
            List<int> triangles = new List<int>();
            for (int i = 0; i < numPoints - 1; ++i)
            {
                int segments = (i < numPoints - 2) ? m_smoothing : m_smoothing + 1;

                for (int s = 0; s < segments; ++s)
                {

                    // output triangles
                    if (startIndex != 0)
                    {
                        //    Debug.Log(startIndex);
                        for (int ii = 0; ii < m_resolution; ++ii)
                        {
                            int curIndex = startIndex + ii;
                            int nextIndex = startIndex + (ii + 1) % m_resolution;

                            triangles.Add(curIndex);
                            triangles.Add(curIndex - m_resolution);
                            triangles.Add(nextIndex - m_resolution);

                            triangles.Add(nextIndex - m_resolution);
                            triangles.Add(nextIndex);
                            triangles.Add(curIndex);
                        }
                    }

                    startIndex += m_resolution;
                }
            }

            trianglesOut = triangles.ToArray();
        }

        public void DrawRope(Transform tr, Particle[] points, int numPoints, ref Vector3[] verticesOut, ref Vector3[] normalsOut, float radius, int resolution, int smoothing)
        {

            if (numPoints < 2)
                return;

            Vector3 u, v;
            Vector3 w = Vector3.Normalize(points[1].pos - points[0].pos);

            BasisFromVector(w, out u, out v);

            Matrix4x4 frame = new Matrix4x4();
            frame.SetColumn(0, new Vector4(u.x, u.y, u.z, 0.0f));
            frame.SetColumn(1, new Vector4(v.x, v.y, v.z, 0.0f));
            frame.SetColumn(2, new Vector4(w.x, w.y, w.z, 0.0f));
            frame.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

            int vId = 0;
            for (int i = 0; i < numPoints - 1; ++i)
            {
                //Vector3 next = new Vector3();

                //if (i > 0)
                //{
                //    if (i < numPoints - 1)
                //        next = Vector3.Normalize(points[i + 1].pos - points[i - 1].pos);
                //    else
                //        next = Vector3.Normalize(points[i].pos - points[i - 1].pos);
                //}

                int a = Mathf.Max(i - 1, 0);
                int b = i;
                int c = Mathf.Min(i + 1, numPoints - 1);
                int d = Mathf.Min(i + 2, numPoints - 1);

                Vector3 p1 = points[b].pos;
                Vector3 p2 = points[c].pos;
                Vector3 m1 = 0.5f * (points[c].pos - points[a].pos);
                Vector3 m2 = 0.5f * (points[d].pos - points[b].pos);

                // ensure last segment handled correctly
                int segments = (i < numPoints - 2) ? smoothing : smoothing + 1;

                for (int s = 0; s < segments; ++s)
                {
                    Vector4 pos = HermiteInterpolate(p1, p2, m1, m2, s / (float)smoothing);
                    Vector3 dir = Vector3.Normalize(HermiteTangent(p1, p2, m1, m2, s / (float)smoothing));

                    //Vector3 cur = frame.GetAxis(2);
                    //Note intentional Vector4.w component drop
                    Vector3 cur = frame.GetColumn(2);
                    float angle = Mathf.Acos(Vector3.Dot(cur, dir));

                    // if parallel then don't need to do anything
                    if (Mathf.Abs(angle) > 0.001f)
                    {
                      //  Quaternion q = Quaternion.AngleAxis(angle, Vector3.Normalize(Vector3.Cross(cur, dir)));
                      //  frame = Matrix4x4.TRS(Vector3.zero, q, Vector3.one) * frame;
                        frame = RotationMatrix(angle, Vector3.Normalize(Vector3.Cross(cur, dir))) * frame;
                    }

                    for (int cc = 0; cc < resolution; ++cc)
                    {
                        float angle2 = 2.0f * Mathf.PI / (float)resolution;

                        // transform position and normal to world space
                        Vector4 vv = frame * new Vector4(Mathf.Cos(angle2 * cc), Mathf.Sin(angle2 * cc), 0.0f, 0.0f);
                        verticesOut[vId] = tr.InverseTransformPoint(vv * radius + pos);
                        normalsOut[vId] = tr.InverseTransformDirection(vv); ;

                        vId++;
                    }

                }
     
            }
        }

      

        // generate a rotation matrix around an axis, from PBRT p74
        Matrix4x4 RotationMatrix(float angle, Vector3 axis)
        {
	        Vector3 a = Vector3.Normalize(axis);
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);

            Matrix4x4 m = new Matrix4x4();

            //m[0, 0] = a.x * a.x + (1.0f - a.x * a.x) * c;
            //m[0, 1] = a.x * a.y * (1.0f - c) + a.z * s;
            //m[0, 2] = a.x * a.z * (1.0f - c) - a.y * s;
            //m[0, 3] = 0.0f;

            //m[1, 0] = a.x * a.y * (1.0f - c) - a.z * s;
            //m[1, 1] = a.y * a.y + (1.0f - a.y * a.y) * c;
            //m[1, 2] = a.y * a.z * (1.0f - c) + a.x * s;
            //m[1, 3] = 0.0f;

            //m[2, 0] = a.x * a.z * (1.0f - c) + a.y * s;
            //m[2, 1] = a.y * a.z * (1.0f - c) - a.x * s;
            //m[2, 2] = a.z * a.z + (1.0f - a.z * a.z) * c;
            //m[2, 3] = 0.0f;

            //m[3, 0] = 0.0f;
            //m[3, 1] = 0.0f;
            //m[3, 2] = 0.0f;
            //m[3, 3] = 1.0f;


            m[0, 0] = a.x * a.x + (1.0f - a.x * a.x) * c;
            m[1, 0] = a.x * a.y * (1.0f - c) + a.z * s;
            m[2, 0] = a.x * a.z * (1.0f - c) - a.y * s;
            m[3, 0] = 0.0f;

            m[0, 1] = a.x * a.y * (1.0f - c) - a.z * s;
            m[1, 1] = a.y * a.y + (1.0f - a.y * a.y) * c;
            m[2, 1] = a.y * a.z * (1.0f - c) + a.x * s;
            m[3, 1] = 0.0f;

            m[0, 2] = a.x * a.z * (1.0f - c) + a.y * s;
            m[1, 2] = a.y * a.z * (1.0f - c) - a.x * s;
            m[2, 2] = a.z * a.z + (1.0f - a.z * a.z) * c;
            m[3, 2] = 0.0f;

            m[0, 3] = 0.0f;
            m[1, 3] = 0.0f;
            m[2, 3] = 0.0f;
            m[3, 3] = 1.0f;

            return m;
            }

        // hermite spline interpolation
        private Vector3 HermiteInterpolate(Vector3 a, Vector3 b, Vector3 t1, Vector3 t2, float t)
        {
            // blending weights
            float w1 = 1.0f - 3 * t * t + 2 * t * t * t;
            float w2 = t * t * (3.0f - 2.0f * t);
            float w3 = t * t * t - 2 * t * t + t;
            float w4 = t * t * (t - 1.0f);

            // return weighted combination
            return a * w1 + b * w2 + t1 * w3 + t2 * w4;
        }

        private Vector3 HermiteTangent(Vector3 a, Vector3 b, Vector3 t1, Vector3 t2, float t)
        {
            // first derivative blend weights
            float w1 = 6.0f * t * t - 6 * t;
            float w2 = -6.0f * t * t + 6 * t;
            float w3 = 3 * t * t - 4 * t + 1;
            float w4 = 3 * t * t - 2 * t;

            // weighted combination
            return a * w1 + b * w2 + t1 * w3 + t2 * w4;
        }

        private void BasisFromVector(Vector3 w, out Vector3 u, out Vector3 v)
        {
            if (Mathf.Abs(w.x) > Mathf.Abs(w.y))
            {
                float invLen = 1.0f / Mathf.Sqrt(w.x * w.x + w.z * w.z);
                u = new Vector3(-w.z * invLen, 0.0f, w.x * invLen);
            }
            else
            {
                float invLen = 1.0f / Mathf.Sqrt(w.y * w.y + w.z * w.z);
                u = new Vector3(0.0f, w.z * invLen, -w.y * invLen);
            }

            v = Vector3.Cross(w, u);


        }

    }
}