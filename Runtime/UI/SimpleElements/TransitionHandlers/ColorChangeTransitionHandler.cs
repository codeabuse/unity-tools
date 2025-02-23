using System.Threading;
using Cysharp.Threading.Tasks;
using Codeabuse.AsyncTools;
#if DOTWEEN
using DG.Tweening;
#endif

namespace Codeabuse.UI
{
    public class ColorChangeTransitionHandler : ColorTransitionHandler
    {
        private CancellationTokenSource _transitionCts = new();

#if UNITASK_ENABLED
        private UniTask _transition;
#else
        private Coroutine _transition;
#endif
        
        protected override void ApplyTransition()
        {
#if DOTWEEN && UNITASK_DOTWEEN_SUPPORT
            _transition = DOTween.To(
                () => TargetGraphic.color, 
                color => TargetGraphic.color = color,
                TargetColor,
                Colors.fadeDuration)
                .ToUniTask(cancellationToken: _transitionCts.Token);
            
#elif UNITASK_ENABLED
            _transition = Tweening.Linear(
                    () => TargetGraphic.color, 
                    value => TargetGraphic.color = value, 
                    TargetColor,
                    Colors.fadeDuration);
            
#else
            _transition = StartCoroutine(TweeningCoroutines.Linear(() => TargetGraphic.color,
                    value => TargetGraphic.color = value,
                    TargetColor,
                    Colors.fadeDuration));
#endif
        }

        
        private void OnEnable()
        {
            if (_transitionCts.IsCancellationRequested)
                _transitionCts = new();
        }

        private void OnDisable()
        {
            if(_transition.Status == UniTaskStatus.Pending)
                _transitionCts.Cancel();
        }
    }
}