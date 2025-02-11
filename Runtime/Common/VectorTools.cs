using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Codeabuse
{
    public static class VectorTools
    {
        public static Vector3 DirectionFromAxis(ArticulationDriveAxis axis) => DirectionFromAxis((int)axis);
        public static Vector3 DirectionFromAxis(Axis axis) => DirectionFromAxis((int)axis);
        public static Vector3 DirectionFromAxis(int axis)
        {
            return new Vector3
            {
                    [axis] = 1
            };
        }

        public static float RadialPosition(Vector3 point, Vector3 center, Vector3 north, Axis plane)
        {
            var normal = Vector3.zero;
            normal[(int)plane] = 1;
            var vector = Vector3.ProjectOnPlane(point - center, normal);
            return Vector3.Angle(north, vector);
        }

        public static Vector3 ClampXYRotation(Vector3 rotationEuler, Vector2 verticalField, Vector2 horizontalField)
        {
            return new Vector3
            {
                    z = rotationEuler.z,
                    x = MathTools.ClampAngle(rotationEuler.x, verticalField.x, verticalField.y),
                    y = MathTools.ClampAngle(rotationEuler.y, horizontalField.x, horizontalField.y)
            };
        }

        public static Vector3 GetAverage(IEnumerable<Vector3> vectors)
        {
            var array = vectors as Vector3[] ?? vectors.ToArray();
            return array.Aggregate(Vector3.zero, (a, b) => a + b, acc => acc / array.Length);
        }

        public static Vector2 xy(this Vector3 vector) => new (vector.x, vector.y);
        public static Vector2 xz(this Vector3 vector) => new (vector.x, vector.z);
        public static Vector2 yz(this Vector3 vector) => new (vector.y, vector.z);
    }
}