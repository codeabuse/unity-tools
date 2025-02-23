using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Codeabuse.UI
{
    public class RadioButton : Toggle
    {
#if UNITY_EDITOR
        //[MenuItem("GameObject/UI/Radio Button")]
        private static void CreateRadioButton()
        {
            var parent = Selection.activeGameObject;
            var root = new GameObject("Radio Button");
            var radioButton = root.AddComponent<RadioButton>();
            var buttonTransform = root.transform as RectTransform;
            if (parent)
            {
                buttonTransform.SetParent(parent.transform);
                buttonTransform.localScale = Vector3.one;
                buttonTransform.localPosition = Vector3.zero;
            }
            var backgroundRoot = new GameObject("Background");
            var background = backgroundRoot.AddComponent<Image>();
            var bgTransform = backgroundRoot.transform as RectTransform;
            bgTransform.transform.SetParent(buttonTransform);
            var highlightRoot = new GameObject("Highlight");
            var highlightImage = highlightRoot.AddComponent<Image>();
            var highlightTransform = highlightRoot.transform as RectTransform;
            highlightTransform.SetParent(bgTransform);
            var knobRoot = new GameObject("Knob");
            var knobImage = knobRoot.AddComponent<Image>();
            var knobTransform = knobRoot.transform as RectTransform;
            bgTransform.localPosition = Vector3.zero;
            bgTransform.localScale = Vector3.one;
            bgTransform.anchorMin = Vector2.zero;
            bgTransform.anchorMax = Vector3.one;
            highlightTransform.localPosition = Vector3.zero;
            highlightTransform.localScale = Vector3.one;
            bgTransform.anchorMin = Vector2.zero;
            bgTransform.anchorMax = Vector3.one;
            var bgWidth = bgTransform.rect.width;
            var horizontalPadding = bgWidth > 90 ? (bgWidth - 90) * .5f : 0;
            var bgRect = bgTransform.rect;
            bgRect.xMin = bgRect.xMax = horizontalPadding;
            
            knobTransform.localPosition = Vector3.zero;
            knobTransform.localScale = Vector3.one;
            backgroundRoot.transform.SetParent(buttonTransform);
            radioButton._knobTransform = knobTransform;
            radioButton._knobImage = knobImage;
            radioButton.image = background;
            radioButton._highlight = highlightImage;
        }
#endif
        
        public RectTransform KnobTransform => _knobTransform;

        public Color InactiveHighlight
        {
            get => _inactiveHighlight;
            set => _inactiveHighlight = value;
        }

        public Color ActiveHighlight
        {
            get => _activeHighlight;
            set => _activeHighlight = value;
        }

        public Color KnobColor
        {
            get => _knobColor;
            set => _knobColor = value;
        }

        public Image Highlight => _highlight;

        public Image KnobImage => _knobImage;

        public Vector3 KnobOnPosition { get; private set; }

        public Vector3 KnobOffPosition { get; private set; }

        [SerializeField]
        private Color _inactiveHighlight = new (.72f, .72f,.72f);
        
        [SerializeField]
        private Color _activeHighlight = new(0, .57f, .7f);
        
        [SerializeField]
        private Color _knobColor = Color.white;
        
        [SerializeField]
        private RectTransform _knobTransform;

        [SerializeField]
        private Image _highlight;
        
        [SerializeField]
        private Image _knobImage;

        private TweenerCore<Vector3, Vector3, VectorOptions> _knobTweener;

        private CancellationTokenSource _knobMovementCancellation = new();
        private UniTask _moveKnobTask;

        protected override void Awake()
        {
            base.Awake();
            InitializeKnobPositions();
        }

        protected override void Start()
        {
            base.Start();
            _highlight.color = isOn ? _activeHighlight : _inactiveHighlight;
            this.onValueChanged.AddListener(HandleValueChanged);
        }

        public void InitializeKnobPositions()
        {
            if (!_knobTransform)
                return;
            var current = _knobTransform.localPosition;
            var opposite = Vector3.Scale(current, new Vector3(-1, 1, 1));
            (KnobOnPosition, KnobOffPosition) = isOn? (current, opposite) : (opposite, current);
        }
        
        public void SetValueWithoutNotify(bool value)
        {
            SetIsOnWithoutNotify(value);
            HandleValueChanged(value);
        }

        private void HandleValueChanged(bool value)
        {
            if (!Application.isPlaying)
            {
                KnobTransform.localPosition = value ? KnobOnPosition : KnobOffPosition;
                _highlight.color = isOn ? _activeHighlight : _inactiveHighlight;
                return;
            }
            if (_moveKnobTask.Status == UniTaskStatus.Pending)
            {
                _knobMovementCancellation.Cancel();
                _knobMovementCancellation = new();
            }

            _moveKnobTask = MoveKnobAsync(value ? KnobOnPosition : KnobOffPosition, _knobMovementCancellation.Token);
        }

        private async UniTask MoveKnobAsync(Vector3 targetLocalPosition, CancellationToken ct)
        {
            await _knobTransform.DOLocalMove(targetLocalPosition, colors.fadeDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
            {
                _highlight.color = isOn ? _activeHighlight : _inactiveHighlight;
            }).ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: ct);
        }
    }
}