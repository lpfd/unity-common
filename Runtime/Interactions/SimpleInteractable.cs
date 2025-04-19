using UnityEngine;
using UnityEngine.Events;

namespace Leap.Forward.Interactions
{
    public class SimpleInteractable : MonoBehaviour, IHoverable, IInteractable
    {
        [SerializeField]
        public string _cursorGlyph = "";

        [SerializeField]
        public string _toolkip = "";

        GameObject IUnityComponent.GameObject => this.gameObject;

        string IHoverable.CursorGlyph => _cursorGlyph;

        string IHoverable.Tooltip => _toolkip;

        public UnityEvent<GameObject> _startInteraction;

        public UnityEvent<GameObject> _stopInteraction;

        public void StartInteraction(GameObject gameObject)
        {
            if (_startInteraction != null)
            {
                _startInteraction.Invoke(gameObject);
            }
        }

        public void StopInteraction(GameObject gameObject)
        {
            if (_stopInteraction != null)
            {
                _stopInteraction.Invoke(gameObject);
            }
        }

        void IHoverable.OnPointerOut()
        {
        }

        void IHoverable.OnPointerOver()
        {
        }
    }
}