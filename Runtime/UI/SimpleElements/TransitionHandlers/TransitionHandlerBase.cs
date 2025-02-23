using UnityEngine;

namespace Codeabuse.UI
{
    public abstract class TransitionHandlerBase : MonoBehaviour, ITransitionHandler
    {
        private bool _needToChangeState = false;
        
        public void ToState(UIControlState state)
        {
            _needToChangeState = true;
            PrepareTransition(state);
        }

        public void Process()
        {
            if (!_needToChangeState)
                return;
            ApplyTransition();
            _needToChangeState = false;
        }

        /// <summary>
        /// Prepare data for given transition mode. Could be called multiple times in one frame.
        /// </summary>
        /// <param name="state"></param>
        protected abstract void PrepareTransition(UIControlState state);
        
        /// <summary>
        /// Called once per frame to apply state transition if needed.
        /// </summary>
        protected abstract void ApplyTransition();
    }
}