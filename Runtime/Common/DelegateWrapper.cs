using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codeabuse
{
    public abstract class DelegateWrapper
    {
        protected readonly Action<object> _action;

        protected DelegateWrapper(Action<object> action)
        {
            _action = action;
        }

        public void Invoke(object value) => _action.Invoke(value);
        //public static implicit operator DelegateWrapper(Action<object> action) => new(action);
    }

    public class DelegateWrapper<T> : DelegateWrapper, IEquatable<DelegateWrapper<T>>
    {
        private readonly Action<T> _genericAction;

        public DelegateWrapper(Action<T> genericAction) : 
                base(x => genericAction.Invoke((T)x))
        {
            _genericAction = genericAction;
        }

        public override bool Equals(object other)
        {
            return other is DelegateWrapper<T> otherWrapper && Equals(otherWrapper);
        }

        public bool Equals(DelegateWrapper<T> other)
        {
            return other is not null && Equals(_genericAction, other._genericAction);
        }

        public override int GetHashCode()
        {
            return (_genericAction != null ? _genericAction.GetHashCode() : 0);
        }

        private string _stringCached;
        public override string ToString()
        {
            return _stringCached ??= GenerateToString();
        }

        private string GenerateToString()
        {
            var method = _genericAction.Method;
            var argsBuilder = new StringBuilder();
            foreach (var parameterInfo in _genericAction.Method.GetParameters())
            {
                argsBuilder.AppendJoin(", ", parameterInfo.ParameterType);
            }
            return $"{method.DeclaringType?.Name ?? "null"}.{method.Name}({argsBuilder})";
        }

        public static implicit operator DelegateWrapper<T>(Action<T> action) => new (action);

        public static explicit operator Action<object>(DelegateWrapper<T> delegateWrapper) =>
                delegateWrapper._action;

        public static implicit operator Action<T>(DelegateWrapper<T> delegateWrapper) =>
                delegateWrapper._genericAction;
    }
    
    public class DelegateWrapperComparer : IEqualityComparer<DelegateWrapper>
    {
        public bool Equals(DelegateWrapper x, DelegateWrapper y)
        {
            return x is not null && y is not null && x.Equals(y);
        }

        public int GetHashCode(DelegateWrapper target)
        {
            return target.GetHashCode();
        }
    }
}