using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;
#endif

namespace Codeabuse.UI
{
    /// <summary>
    /// Lightweight UI interactable class, allows to handle multiple target graphics via <see cref="ITransitionHandler"/>.
    /// Supports multiple cursors (i.e. in VR and multiplayer).
    /// </summary>
    public class SimpleInteractable : MonoBehaviour,
            IPointerDownHandler,
            IPointerUpHandler,
            IPointerClickHandler,
            IPointerEnterHandler,
            IPointerExitHandler,
            IControlElement
    {
        [SerializeField]
        protected bool _interactable = true;
        
        [SerializeField]
        private UnityEvent<UIControlState> _onStateTransition;

        [SerializeField]
        private UIControlState _controlState = UIControlState.Normal;

        private readonly List<ITransitionHandler> _transitionHandlers = new();
        
        private readonly HashSet<int> _hoveredBy = new();
        
#if UNITASK_ENABLED
        private CancellationTokenSource _trackSelectableCancellation = new();
#endif

        public bool IsHovered => _hoveredBy.Count != 0;
        
        
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

        protected virtual void Awake()
        {
            _transitionHandlers.AddRange(GetComponents<ITransitionHandler>());
        }

        private void TrackInteractableState(Selectable selectable)
        {
            var state = selectable.interactable;
#if UNITASK_ENABLED
            UniTask.Create(async (ct) =>
            {
                await UniTask.WaitUntil(() => selectable.interactable != state, cancellationToken: ct);
                this.Interactable = selectable.interactable;
                TrackInteractableState(selectable);
            },  this.GetCancellationTokenOnDestroy());
#else
            StartCoroutine(TrackInteractableStateCoroutine());
            
            IEnumerator TrackInteractableStateCoroutine()
            {
                while (this && this.enabled)
                {
                    yield return null;
                    if (selectable.interactable != state)
                    {
                        state = this.Interactable = selectable.interactable;
                    }
                }
            }
#endif
        }
        public void AddTransitionHandler(ITransitionHandler handler) => _transitionHandlers.Add(handler);
        public void RemoveTransitionHandler(ITransitionHandler handler) => _transitionHandlers.Remove(handler);

        public void ApplyTransition(UIControlState state)
        {
            if (state == _controlState)
                return;

            ApplyTransitionWithoutNotify(state);
            OnStateTransition.Invoke(state);
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

        protected virtual void OnEnable()
        {
            ApplyTransition(_interactable? UIControlState.Normal : UIControlState.Disabled);
            if (GetComponent<Selectable>() is {} selectable)
            {
                TrackInteractableState(selectable);
            }
#if UNITASK_ENABLED
            if (_trackSelectableCancellation.IsCancellationRequested)
            {
                _trackSelectableCancellation = new();
            }
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITASK_ENABLED
            _trackSelectableCancellation.Cancel();
#endif
        }

        protected virtual void Update()
        {
            foreach (var transitionHandler in _transitionHandlers)
            {
                transitionHandler.Process();
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            
            var state = IsHovered ? UIControlState.Hovered : UIControlState.Normal;
            ApplyTransition(state);
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_interactable) 
                return;
            
            var wasHovered = IsHovered;
            _hoveredBy.Add(eventData.pointerId);
            
            if(!wasHovered)
                ApplyTransition(UIControlState.Hovered);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_interactable) 
                return;
            
            _hoveredBy.Remove(eventData.pointerId);
            
            if (IsHovered)
                return;
            
            ApplyTransition(UIControlState.Normal);
        }
    }
}