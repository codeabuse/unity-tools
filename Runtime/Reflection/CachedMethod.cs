using System;
using System.Reflection;

namespace Codeabuse
{
    public readonly struct CachedMethod
    {
        private readonly object _target;
        private readonly MethodInfo _targetMethod;
        private readonly Func<object[]> _getArguments;

        public CachedMethod(object target, MethodInfo targetMethod, Func<object[]> getArguments)
        {
            _target = target;
            _targetMethod = targetMethod;
            _getArguments = getArguments;
        }

        public void Invoke()
        {
            _targetMethod.Invoke(_target, _getArguments());
        }
    }
}