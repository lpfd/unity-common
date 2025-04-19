using UnityEngine;
using UnityEngine.Events;

namespace Leap.Forward.Interactions
{
    public class RayCastInteractionHandler : MonoBehaviour, IInteractionHandler
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
        private ActiveInteraction activeInteraction;

        public class ActiveInteraction
        {
            public IInteractable Interactable { get; set; }
            public GameObject Target { get; set; }
        }

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
                    StopInteraction();

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

        public void StartInteraction(GameObject gameObject)
        {
            StopInteraction();
            if (lastHoverable != null)
            {
                var activeInteractable = lastHoverable.GameObject.GetComponentInParent<IInteractable>();
                if (activeInteractable != null)
                {
                    activeInteraction = new ActiveInteraction { Interactable = activeInteractable, Target = gameObject };
                    activeInteractable.StartInteraction(gameObject);
                }
            }
        }

        public void StopInteraction(GameObject gameObject)
        {
            StopInteraction();
        }

        private void StopInteraction()
        {
            if (activeInteraction != null)
            {
                activeInteraction.Interactable.StopInteraction(activeInteraction.Target);
                activeInteraction = null;
            }
        }
    }
}