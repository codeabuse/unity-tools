using System;
using System.Collections.Generic;

namespace Codeabuse
{
    public readonly struct ObjectPair<T> : IEquatable<ObjectPair<T>>
    {
        public static IEqualityComparer<ObjectPair<T>> Comparer { get; } = new ObjectsPairComparer();
        
        private readonly T _left;
        private readonly T _right;

        public T Left => _left;
        public T Right => _right;
        
        public ObjectPair(T left, T right)
        {
            _left = left;
            _right = right;
        }

        public bool Equals(ObjectPair<T> other)
        {
            return (_left.Equals(other._left) && _right.Equals(other._right)) ||
                   (_left.Equals(other._right) && _right.Equals(other._left));
        }

        public override bool Equals(object obj)
        {
            if (obj is null) 
                return false;
            return obj.GetType() == this.GetType() && 
                   Equals((ObjectPair<T>)obj);
        }

        public override int GetHashCode()
        {
            return _left.GetHashCode() ^ _right.GetHashCode();
        }

        private class ObjectsPairComparer : IEqualityComparer<ObjectPair<T>>
        {
            public bool Equals(ObjectPair<T> x, ObjectPair<T> y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ObjectPair<T> obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}