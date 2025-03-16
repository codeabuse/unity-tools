#if UNITASK_ENABLED
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Codeabuse.AsyncTools
{
    public static class Tweening
    {
        public static async UniTask Evaluate(Func<Color> getter, Action<Color> setter, Color target, float duration, Func<float, float> curve)
        {
            var time = 0f;
            while (time < duration)
            {
                await UniTask.DelayFrame(1);
                time += Time.deltaTime;
                setter(Color.Lerp(getter(), target, curve(time / duration)));
            }

            setter(target);
        }

        public static UniTask Linear(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Linear);
        }
        
        public static UniTask Quadratic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quadratic);
        }
        
        public static UniTask Cubic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Cubic);
        }
        
        public static UniTask Quintic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.Quintic);
        }
        
        public static UniTask InverseQuadratic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuadratic);
        }
        
        public static UniTask InverseCubic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseCubic);
        }
        
        public static UniTask InvesreQuintic(Func<Color> getter, Action<Color> setter, Color target, float duration)
        {
            return Evaluate(getter, setter, target, duration, CurveFunctions.InverseQuintic);
        }

    }
}
#endif
