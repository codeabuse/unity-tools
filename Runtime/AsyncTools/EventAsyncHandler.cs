#if UNITASK_ENABLED
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Codeabuse.AsyncTools
{
    /// <summary>
    /// Allows to await delegates. Use extension method <see cref="WaitForEvent"/>
    /// to await events (provide subscribe and unsubscribe delegates as lambdas).
    /// </summary>
    public class EventAsyncHandler : IUniTaskSource, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly Action _onComplete;
        readonly Action<Action> _removeInvokeDelegate;

        readonly CancellationToken cancellationToken;
        readonly CancellationTokenRegistration registration;
        bool isDisposed;
        readonly bool callOnce;
        
        UniTaskCompletionSourceCore<AsyncUnit> core;
        
        public EventAsyncHandler(Action<Action> addInvokeDelegate, 
                Action<Action> removeInvokeDelegate, CancellationToken ct, bool callOnce)
        {
            this.cancellationToken = ct;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this._onComplete = Invoke;
            addInvokeDelegate(_onComplete);
            _removeInvokeDelegate = removeInvokeDelegate;
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
                _removeInvokeDelegate.Invoke(_onComplete);
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

        public static UniTask WaitForEvent(
                Action<Action> subscribe, 
                Action<Action> unsubscribe, 
                CancellationToken cancellationToken)
        {
            var eventHandler = new EventAsyncHandler(
                    subscribe, 
                    unsubscribe,
                    cancellationToken, 
                    true);
            
            return eventHandler.OnInvokeAsync();
        }
    }
    
    public class EventAsyncHandler<T> : IUniTaskSource<T>, IAsyncClickEventHandler
    {
        static Action<object> cancellationCallback = CancellationCallback;

        readonly Action<T> _onComplete;
        readonly Action<Action<T>> _removeInvokeDelegate;

        readonly CancellationToken cancellationToken;
        readonly CancellationTokenRegistration registration;
        bool isDisposed;
        readonly bool callOnce;

        private UniTaskCompletionSourceCore<T> core;

        public EventAsyncHandler(Action<Action<T>> addInvokeDelegate, 
                Action<Action<T>> removeInvokeDelegate, CancellationToken ct, bool callOnce)
        {
            this.cancellationToken = ct;
            if (cancellationToken.IsCancellationRequested)
            {
                isDisposed = true;
                return;
            }

            this._onComplete = Invoke;
            addInvokeDelegate(_onComplete);
            _removeInvokeDelegate = removeInvokeDelegate;
            this.callOnce = callOnce;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.RegisterWithoutCaptureExecutionContext(cancellationCallback, this);
            }

            TaskTracker.TrackActiveTask(this, 3);
        }
        
        void Invoke(T value)
        {
            core.TrySetResult(value);
        }
        
        public UniTask<T> OnInvokeAsync()
        {
            core.Reset();
            if (isDisposed)
            {
                core.TrySetCanceled(this.cancellationToken);
            }
            return new UniTask<T>(this, core.Version);
        }
        
        T IUniTaskSource<T>.GetResult(short token)
        {
            try
            {
                return core.GetResult(token);
            }
            finally
            {
                if (callOnce)
                {
                    Dispose();
                }
            }
        }
        
        void IUniTaskSource.GetResult(short token)
        {
            ((IUniTaskSource<T>)this).GetResult(token);
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
                _removeInvokeDelegate?.Invoke(_onComplete);
                core.TrySetCanceled(cancellationToken);
            }
        }
        
        UniTask IAsyncClickEventHandler.OnClickAsync()
        {
            return OnInvokeAsync();
        }
        
        static void CancellationCallback(object state)
        {
            var self = (EventAsyncHandler<T>)state;
            self.Dispose();
        }
        
        public static UniTask<T> WaitForEvent(
                Action<Action<T>> subscribe, 
                Action<Action<T>> unsubscribe,
                CancellationToken cancellationToken)
        {
            var eventHandler = new EventAsyncHandler<T>(
                    subscribe, 
                    unsubscribe,
                    cancellationToken, 
                    true);
            return eventHandler.OnInvokeAsync();
        }
    }
}
#endif