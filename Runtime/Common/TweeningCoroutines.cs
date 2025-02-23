using System;
using System.Collections;
using UnityEngine;

namespace Codeabuse
{
    public static class TweeningCoroutines
    {
        private static readonly WaitForEndOfFrame waitForEndOfFrame = new();
        
        public static IEnumerator Evaluate(Func<Color> getter, Action<Color> setter, Color target, float duration, Func<float, float> curve)
        {
            var time = 0f;
            while (time < duration)
            {
                yield return waitForEndOfFrame;
                time += Time.deltaTime;
                setter(Color.Lerp(getter(), target, curve(time / duration)));
            }

            setter(target);
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