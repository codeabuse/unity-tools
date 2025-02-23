
namespace Codeabuse.UI
{
    public class ColorTintTransitionHandler : ColorTransitionHandler
    {
        protected override void ApplyTransition()
        {
            TargetGraphic.CrossFadeColor(TargetColor, _colors.fadeDuration, true, true);
        }
    }
}