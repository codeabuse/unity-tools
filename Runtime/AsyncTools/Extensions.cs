using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Codeabuse.AsyncTools
{
    public static class Extensions
    {
        public static UniTask WaitFor(this Action action, CancellationToken ct)
        {
            var eventHandler = new EventAsyncHandler(invoke => action += invoke,
                    invoke => action -= invoke,
                    ct, true);
            return eventHandler.OnInvokeAsync();
        }
    }
}