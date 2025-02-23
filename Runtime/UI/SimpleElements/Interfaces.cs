using System;
using UnityEngine.Events;

namespace Codeabuse.UI
{
    public interface IDigitTextParser<TValue> where TValue: struct
    {
        UnityEvent<TValue> OnValueChanged { get; }
        TValue Value { get; }
    }
    
    public interface IExitButtonController
    {
        public event Action<string> OnTextChanged;
        void UpdateButtonText();
        void OnButtonClicked();
    }
}