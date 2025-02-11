using System;

namespace Codeabuse
{
    public readonly struct Result<TValue, TError>
    {
        private readonly TValue _value;
        private readonly TError _error;
        private readonly bool _successful;
        public bool Successful => _successful;
        
        public Result(TValue value)
        {
            _value = value;
            _error = default;
            _successful = true;
        }

        public Result(TError error)
        {
            _value = default;
            _error = error;
            _successful = true;
        }

        public void Map(Action<TValue> onSuccess, Action<TError> onFail)
        {
            if (_successful)
                onSuccess(_value);
            else
                onFail(_error);
        }

        public static implicit operator Result<TValue, TError>(TValue value) => new(value);
        public static implicit operator Result<TValue, TError>(TError error) => new(error);
    }
    
    public readonly struct Result<TValue>
    {
        private readonly TValue _value;
        private readonly string _error;
        private readonly bool _successful;
        public bool Successful => _successful;

        public Result(TValue value)
        {
            _value = value;
            _error = default;
            _successful = true;
        }

        public Result(string error)
        {
            _value = default;
            _error = error;
            _successful = false;
        }

        public void Map(Action<TValue> onSuccess, Action<string> onFail)
        {
            if (_successful)
                onSuccess(_value);
            else
                onFail(_error);
        }

        public static implicit operator Result<TValue>(TValue value) => new(value);
        public static implicit operator Result<TValue>(string error) => new(error);
    }
}