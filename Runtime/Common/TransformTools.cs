using System;
using UnityEngine;

namespace Codeabuse
{
    public static class TransformTools
    {
        /// <summary>
        /// Get the azimuth from <param name="center"></param> transform to <param name="target"></param>
        /// relative to local <param name="planeNormal"></param>'s north:
        /// X normal = YZ plane = Up is north,
        /// Y normal = XZ plane = Forward is north,
        /// Z normal = ZY plane = Right is north.
        /// </summary>
        /// <returns> Angle in degrees </returns>
        /// <exception cref="ArgumentOutOfRangeException">Invalid Axis value</exception>
        public static float GetAzimuth(this Transform target, Transform center, Axis planeNormal)
        {
            var north = planeNormal switch {
                    Axis.X => center.up,
                    Axis.Y => center.forward,
                    Axis.Z => center.right,
                    _ => throw new ArgumentOutOfRangeException(nameof(planeNormal), planeNormal, "Invalid Axis value")
            };
            return VectorTools.RadialPosition(target.position, center.position, north, planeNormal);
        }
        
        public static float GetAzimuth(this Rigidbody target, Transform center, Axis planeNormal)
        {
            return GetAzimuth(target.transform, center, planeNormal);
        }

        public static Vector3 Direction(this Transform transform, Axis axis) => axis switch
        {
                Axis.X => transform.right,
                Axis.Y => transform.up,
                Axis.Z => transform.forward,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Invalid Axis value")
        };
    }
}