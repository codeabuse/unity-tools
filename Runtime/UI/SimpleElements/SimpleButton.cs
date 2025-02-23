using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Codeabuse.UI
{
    /// <summary>
    /// Lightweight Button class, much more flexible than Unity UGUI button.
    /// Supports multiple target graphics via <see cref="TransitionHandlerBase"/>.
    /// Supports multiple cursors (i.e. in VR and multiplayer).
    /// </summary>
    public class SimpleButton : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IControlElement
    {
        [SerializeField]
        private ClickEventType _clickEventType;
        [SerializeField]
        private bool _interactable = true;
        [SerializeField]
        private UnityEvent _onClick;

        [SerializeField]
        private List<TransitionHandlerBase> _transitionHandlers = new();
        [SerializeField]
        private UnityEvent<UIControlState> _onStateTransition;

        [SerializeField]
        private UIControlState _controlState = UIControlState.Normal;

        private readonly HashSet<int> _hoveredBy = new();
        private readonly HashSet<int> _pressedBy = new();

        public bool IsHovered => _hoveredBy.Count != 0;
        public bool IsPressed => _pressedBy.Count != 0;

        public UnityEvent OnClick => _onClick;

        public bool Interactable
        {
            get => _interactable;
            set
            {
                if (value != _interactable)
                {
                    ApplyTransition(value? UIControlState.Normal : UIControlState.Disabled);
                }
                _interactable = value;
            }
        }

        public UnityEvent<UIControlState> OnStateTransition => _onStateTransition;
        public UIControlState ControlState => _controlState;
        
        public void ApplyTransition(UIControlState state)
        {
            if (state == _controlState)
                return;

            ApplyTransitionWithoutNotify(state);
            _onStateTransition.Invoke(state);
        }
        
        public void ApplyTransitionWithoutNotify(UIControlState state)
        {
            if (state == _controlState)
                return;
            
            _controlState = state;
            foreach (var transitionHandler in _transitionHandlers)
            {
                transitionHandler.ToState(state);
            }
        }

        public void SetInteractableWithoutNotify(bool value)
        {
            _interactable = value;
            ApplyTransitionWithoutNotify(_interactable? UIControlState.Normal : UIControlState.Disabled);
        }

        private void Start()
        {
            ApplyTransition(_interactable? UIControlState.Normal : UIControlState.Disabled);
        }

        private void OnEnable()
        {
            ApplyTransition(_interactable? UIControlState.Normal : UIControlState.Disabled);
        }

        private void OnDisable()
        {
            ApplyTransition(UIControlState.Disabled);
            foreach (var transitionHandler in _transitionHandlers)
            {
                transitionHandler.Process();
            }
        }

        private void Update()
        {
            foreach (var transitionHandler in _transitionHandlers)
            {
                transitionHandler.Process();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable) return;
            _pressedBy.Add(eventData.pointerId);
            if (_clickEventType is not ClickEventType.PointerDown)
                return;
            _onClick.Invoke();
            ApplyTransition(UIControlState.Pressed);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            _pressedBy.Remove(eventData.pointerId);

            if (_clickEventType is ClickEventType.PointerUp)
            {
                if (IsPressed) return;
                _onClick.Invoke();
            }
            var state = IsHovered ? UIControlState.Hovered : UIControlState.Normal;
            ApplyTransition(state);
        }
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            if (_clickEventType is not ClickEventType.PointerClick)
                return;
            _onClick.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (!_interactable) return;
            var wasHovered = IsHovered;
            _hoveredBy.Add(eventData.pointerId);
            if(!wasHovered)
                ApplyTransition(UIControlState.Hovered);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (!_interactable) return;
            _hoveredBy.Remove(eventData.pointerId);
            if (IsHovered)
                return;
            ApplyTransition(UIControlState.Normal);
        }
    }
}