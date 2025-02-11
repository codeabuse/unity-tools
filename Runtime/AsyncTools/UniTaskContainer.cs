#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Codeabuse.AsyncTools
{
    public delegate UniTask UniTaskWithPropagateCancellation(CancellationToken ctx);
    
    public class UniTaskContainer
    {
        private UniTask _task;
        private CancellationTokenSource _cts;

        public UniTaskStatus Status => _task.Status;
        public UniTask Task => _task;
        public bool IsRunning => _task.Status is UniTaskStatus.Pending;
        public bool IsCancelled => _cts is { IsCancellationRequested: true };

        public UniTask Start(UniTaskWithPropagateCancellation createTask)
        {
            if (_cts is null or { IsCancellationRequested: true })
            {
                _cts = new();
            }
            return _task = createTask(_cts.Token);
        }

        public UniTask Start(in UniTask task)
        {
            if (_cts is null or { IsCancellationRequested: true })
            {
                _cts = new();
            }
            return _task = task.AttachExternalCancellation(_cts.Token);
        }

        public bool Cancel()
        {
            if (_cts is null or { IsCancellationRequested: true })
            {
                return false;
            }
            
            _cts.Cancel();
            return true;
        }

        public void CancelAndRefresh()
        {
            Cancel();
            _cts = new();
        }
    }
}
#endif