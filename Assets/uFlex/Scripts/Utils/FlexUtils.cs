using UnityEngine;

using System;
using System.Runtime.InteropServices;


namespace uFlex
{
    /// <summary>
    /// Various handy utilities
    /// </summary>
    public class FlexUtils
    {

        public static T[] MarshallArrayOfStructures<T>(IntPtr ptr, int n)
        {
            int structSize = Marshal.SizeOf(typeof(T));
            T[] output = new T[n];

            for (int i = 0; i < n; i++)
            {
                IntPtr data = new IntPtr(ptr.ToInt64() + structSize * i);
                T t = (T)Marshal.PtrToStructure(data, typeof(T));
                output[i] = t;
            }
            return output;
        }

        // component wise min max functions
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public static void GetBounds(Vector3[] points, out Vector3 outMinExtents, out Vector3 outMaxExtents)
        {
            Vector3 minExtents = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 maxExtents = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // calculate face bounds
            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 a = points[i];

                minExtents = Min(a, minExtents);
                maxExtents = Max(a, maxExtents);
            }

            outMinExtents = minExtents;
            outMaxExtents = maxExtents;
        }

        //public static void TransformBounds(Vector3 localLower, Vector3 localUpper, Vector3 translation, Quaternion rotation, float scale, out Vector3 lower, out Vector3 upper)
        //{
        // //Matrix33 transform(rotation);

        // //   Vector3 extents = (localUpper - localLower) * scale;

        // //   transform.cols[0] *= extents.x;
        // //transform.cols[1] *= extents.y;
        // //transform.cols[2] *= extents.z;

        // float ex = fabsf(transform.cols[0].x) + fabsf(transform.cols[1].x) + fabsf(transform.cols[2].x);
        //    float ey = fabsf(transform.cols[0].y) + fabsf(transform.cols[1].y) + fabsf(transform.cols[2].y);
        //    float ez = fabsf(transform.cols[0].z) + fabsf(transform.cols[1].z) + fabsf(transform.cols[2].z);

        // //   Vector3 center = (localUpper + localLower) * 0.5f * scale;

        //    lower = rotation* center + translation - new Vector3(ex, ey, ez) * 0.5f;
        // upper = rotation* center + translation + new Vector3(ex, ey, ez) * 0.5f;
        //}

        // calculates local space positions given a set of particles and rigid indices
        public static void CalculateRigidOffsets(Vector3[] restPositions, int[] offsets, int[] indices, int numRigids, Vector3[] localPositions)
        {
            int count = 0;

            for (int r = 0; r < numRigids; ++r)
            {
                int startIndex = offsets[r];
                int endIndex = offsets[r + 1];

                int n = endIndex - startIndex;

                Vector3 com = new Vector3();

                for (int i = startIndex; i < endIndex; ++i)
                {

                    com += restPositions[indices[i]];
                }

                com /= (float)n;

                for (int i = startIndex; i < endIndex; ++i)
                {
                    localPositions[count++] = (restPositions[indices[i]]) - com;
                }
            }
        }
    }


}