using UnityEngine;
using UnityEngine.Events;

namespace Leap.Forward.Interactions
{
    public class RayCastInteractionHandler : MonoBehaviour
    {
        [System.Serializable]
        public class HoverEvent : UnityEvent<IHoverable> { }

        [Tooltip("Ray emitter for raycast (optinal).")]
        public Transform rayEmitter;

        [Tooltip("Raycast distance.")]
        public float raycastDistance;

        [Tooltip("Interaction layer mask.")]
        public LayerMask layerMask;

        [Tooltip("Colliders interaction mode.")]
        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        [Header("Events")]

        [SerializeField]
        public HoverEvent onPointerOver;

        [SerializeField]
        public HoverEvent onPointerOut;

        private GameObject lastTarget;

        private IHoverable lastHoverable;

        public void Awake()
        {
            
        }

        public void Update()
        {
            var emitter = rayEmitter ?? transform;
            var ray = new Ray(rayEmitter.position, rayEmitter.forward);

            if (Physics.Raycast(ray, out var raycastHit, raycastDistance, layerMask, queryTriggerInteraction))
            {
                var hitCollider = raycastHit.collider;

                //if (lastTarget == hitCollider.gameObject)
                //{
                //    return;
                //}
                lastTarget = hitCollider.gameObject;
                var hoverable = lastTarget.GetComponentInParent<IHoverable>();
                if (hoverable != lastHoverable)
                {
                    if (lastHoverable != null)
                    {
                        lastHoverable.OnPointerOut();
                        onPointerOut?.Invoke(lastHoverable);
                    }
                    lastHoverable = hoverable;
                    if (lastHoverable != null)
                    {
                        lastHoverable.OnPointerOver();
                        onPointerOver?.Invoke(lastHoverable);
                    }
                }
                return;
            }

            if (null != lastHoverable)
            {
                lastHoverable?.OnPointerOut();
                onPointerOut?.Invoke(lastHoverable);
                lastHoverable = null;
            }
        }
    }
}