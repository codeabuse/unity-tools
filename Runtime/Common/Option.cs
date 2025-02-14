using System;

namespace Codeabuse
{
    /// <summary>
    /// Represents optional result, useful to replace null return type or nullable value types,
    /// providing safe execution path when no value has been returned.
    /// Can be used in combination with Result.
    /// <value>null</value> is usually implicitly converted into <value>Option.None</value>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public readonly struct Option<TValue>
    {
        public static Option<TValue> None => new(false);

        public bool HasValue => _hasValue;

        private readonly TValue _value;
        private readonly bool _hasValue;

        public Option(TValue value)
        {
            _value = value;
            _hasValue = true;
        }

        private Option(bool hasValue)
        {
            _value = default;
            _hasValue = hasValue;
        }

        public void Match(Action<TValue> some, Action none = null)
        {
            if (_hasValue)
            {
                some(_value);
            }
            else
            {
                none?.Invoke();
            }
        }

        public static implicit operator Option<TValue>(TValue value)
        {
            return value is {}?
                new (value) :
                None;
        }
    }
}