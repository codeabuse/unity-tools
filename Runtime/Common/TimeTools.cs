using System;
using UnityEngine;

namespace Codeabuse
{
    public static class TimeTools
    {
        public static float Delta(UpdateLoop timeLoop) => timeLoop switch
        {
                UpdateLoop.Dynamic => Time.deltaTime,
                UpdateLoop.Fixed => Time.fixedDeltaTime,
                UpdateLoop.Unscaled => Time.unscaledDeltaTime,
                _ => throw new ArgumentOutOfRangeException(nameof(timeLoop), timeLoop, null)
        };
    }
}