using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Codeabuse
{
    public class ContextFunction
    {
        public string Label { get; }
        private readonly Object _target;
        private readonly MethodInfo _methodInfo;

        public ContextFunction(Object target, MethodInfo methodInfo, string label)
        {
            _target = target;
            _methodInfo = methodInfo;
            Label = label;
        }

        public void Invoke()
        {
            _methodInfo.Invoke(_target, Array.Empty<object>());
        }
    }
}