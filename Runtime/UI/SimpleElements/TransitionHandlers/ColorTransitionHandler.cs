using UnityEngine;
using UnityEngine.UI;

namespace Codeabuse.UI
{
    public abstract class ColorTransitionHandler : TransitionHandlerBase
    {
        [SerializeField]
        private Graphic _targetGraphic;

        [SerializeField]
        protected ColorBlock _colors = ColorBlock.defaultColorBlock;

        private Color _targetColor;

        public Color TargetColor
        {
            get => _targetColor;
            set => _targetColor = value;
        }

        protected ColorBlock Colors => _colors;

        public Graphic TargetGraphic => _targetGraphic;

        protected override void PrepareTransition(UIControlState state)
        {
            _targetColor = state switch
            {
                UIControlState.Normal => _colors.normalColor,
                UIControlState.Hovered => _colors.highlightedColor,
                UIControlState.Pressed => _colors.pressedColor,
                UIControlState.Selected => _colors.selectedColor,
                UIControlState.Disabled => _colors.disabledColor
            };
        }
    }
}