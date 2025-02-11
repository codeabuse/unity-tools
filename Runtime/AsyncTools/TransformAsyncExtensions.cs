#if UNITASK_ENABLED
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Codeabuse.AsyncTools
{
    public static class TransformAsyncExtensions
    {
        private const float angle_threshold = .05f;
        
        public static async UniTask MoveTowards(
                this Transform transform, 
                Transform target, 
                float maxDelta,
                bool matchRotation,
                CancellationToken ctx,
                float snapDistance = 0.05f,
                float maxAngleDelta = 30f)
        {
            var distance = (transform.position - target.position).magnitude;
            while (distance > snapDistance && !matchRotation || Quaternion.Angle(transform.rotation, target.rotation) > angle_threshold)
            {
                var deltaTime = TimeTools.Delta(UpdateLoop.Dynamic);
                var position = transform.position;
                var targetPosition = target.position;
                position = Vector3.MoveTowards(position, targetPosition, maxDelta * deltaTime);
                transform.position = position;
                if (matchRotation)
                {
                    var targetRotation = target.rotation;
                    var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                            maxAngleDelta * deltaTime);
                    transform.rotation = rotation;
                }
                distance = (position - targetPosition).magnitude;
                await UniTask.DelayFrame(1, cancellationToken: ctx);
            }

            transform.position = target.position;
            if (matchRotation)
                transform.rotation = target.rotation;
        }
    }
}
#endif