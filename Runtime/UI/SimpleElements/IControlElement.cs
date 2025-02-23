using UnityEngine.Events;

namespace Codeabuse.UI
{
    public interface IControlElement
    {
        bool Interactable { get; set; }
        UnityEvent<UIControlState> OnStateTransition { get; }
        void ApplyTransition(UIControlState state);
        void ApplyTransitionWithoutNotify(UIControlState state);
        void SetInteractableWithoutNotify(bool value);
    }
}