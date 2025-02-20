using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace Codeabuse.AsyncTools
{
    public static class VisualElementsAsyncExtensions
    {
        public static UniTask AsyncWaitForCLick(this Button button, Action callback, CancellationToken ct)
        {
            button.clicked += callback;
            
            return new EventAsyncHandler(action => button.clicked += action,
                    action => button.clicked -= action,
                    ct, false).OnInvokeAsync();
        }
        
        public static UniTask AsyncWaitForCLick(this Button button, CancellationToken ct)
        {
            
            return new EventAsyncHandler(action => button.clicked += action,
                    action => button.clicked -= action,
                    ct, true).OnInvokeAsync();
        }
    }
}