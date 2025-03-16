using System;
using System.Collections;
using UnityEngine;

namespace Codeabuse
{
    public static class TweeningCoroutines
    {
        private static readonly WaitForEndOfFrame waitForEndOfFrame = new();
        
        public static IEnumerator Evaluate(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration, Func<float, float> curve)
        {
            var time = 0f;
            var start = getter();
            while (time < duration)
            {
                yield return waitForEndOfFrame;
                time += Time.deltaTime;
                setter(Vector3.Lerp(start, target, curve(time / duration)));
            }

            setter(target);
        }
        public static IEnumerator Evaluate(Func<Color> getter, Action<Color> setter, Color target, float duration, Func<float, float> curve)
        {
            var time = 0f;
            var start = getter();
            while (time < duration)
            {
                yield return waitForEndOfFrame;
                time += Time.deltaTime;
                setter(Color.Lerp(start, target, curve(time / duration)));
            }

            setter(target);
        }

        public static IEnumerator Linear(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Linear);
        }

        public static IEnumerator Quadratic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quadratic);
        }
        
        public static IEnumerator Cubic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Cubic);
        }
        
        public static IEnumerator Quintic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quintic);
        }
        
        public static IEnumerator InverseQuadratic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuadratic);
        }
        
        public static IEnumerator InverseCubic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseCubic);
        }
        
        public static IEnumerator InvesreQuintic(Func<Vector3> getter, Action<Vector3> setter, Vector3 target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuintic);
        }
        
        public static IEnumerator Linear(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Linear);
        }

        public static IEnumerator Quadratic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quadratic);
        }
        
        public static IEnumerator Cubic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Cubic);
        }
        
        public static IEnumerator Quintic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quintic);
        }
        
        public static IEnumerator InverseQuadratic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuadratic);
        }
        
        public static IEnumerator InverseCubic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseCubic);
        }
        
        public static IEnumerator InvesreQuintic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuintic);
        }
    }
}