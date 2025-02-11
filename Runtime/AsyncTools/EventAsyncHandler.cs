#if UNITASK_ENABLED
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Codeabuse
{
    public class EventAsyncHandler : IUniTaskSource, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly Action _action;
        readonly Action<Action> _unsubscribeFromTargetEvent;

        readonly CancellationToken cancellationToken;
        readonly CancellationTokenRegistration registration;
        bool isDisposed;
        readonly bool callOnce;
        
        UniTaskCompletionSourceCore<AsyncUnit> core;

        public EventAsyncHandler(Action<Action> subscribeToTargetEvent, 
                Action<Action> unsubscribeFromTargetEvent, CancellationToken ct, bool callOnce)
        {
            this.cancellationToken = cancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this._action = Invoke;
            subscribeToTargetEvent(_action);
            _unsubscribeFromTargetEvent = unsubscribeFromTargetEvent;
            this.callOnce = callOnce;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }
        
        void Invoke()
        {
            core.TrySetResult(AsyncUnit.Default);
        }
        
        public UniTask OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new UniTask(this, core.Version);
        }
        
        void IUniTaskSource.GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }
        
        UniTaskStatus IUniTaskSource.GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        UniTaskStatus IUniTaskSource.UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        void IUniTaskSource.OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                TaskTracker.RemoveTracking(this);
                registration.Dispose();
                _unsubscribeFromTargetEvent?.Invoke(_action);
                core.TrySetCanceled(cancellationToken);
            }
        }
        
        UniTask IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }
        
        static void CancellationCallback(object state)
        {
            var self = (EventAsyncHandler)state;
            self.Dispose();
        }
    }
}
#endif