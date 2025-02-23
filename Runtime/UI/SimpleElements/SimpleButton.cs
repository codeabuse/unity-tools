using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Codeabuse.UI
{
    /// <summary>
    /// Lightweight Button class.
    /// </summary>
    public class SimpleButton : SimpleInteractable
    {
        [SerializeField]
        private ClickEventType _clickEventType;
        [SerializeField]
        private UnityEvent _onClick;
        

        private readonly HashSet<int> _pressedBy = new();

        public bool IsPressed => _pressedBy.Count != 0;

        public UnityEvent OnClick => _onClick;
     
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable) 
                return;
            
            _pressedBy.Add(eventData.pointerId);
            
            if (_clickEventType is not ClickEventType.PointerDown)
                return;
            
            _onClick.Invoke();
            ApplyTransition(UIControlState.Pressed);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) 
                return;
            
            _pressedBy.Remove(eventData.pointerId);

            if (_clickEventType is ClickEventType.PointerUp)
            {
                if (IsPressed) return;
                _onClick.Invoke();
            }
            
            base.OnPointerUp(eventData);
        }
        
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) 
                return;
            
            if (_clickEventType is not ClickEventType.PointerClick)
                return;
            
            _onClick.Invoke();
        }
    }
}