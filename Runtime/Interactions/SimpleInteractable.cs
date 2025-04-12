using UnityEngine;

namespace Leap.Forward.Interactions
{
    public class SimpleInteractable : MonoBehaviour, IHoverable, IInteractable
    {
        [SerializeField]
        public string _cursorGlyph = "";

        [SerializeField]
        public string _toolkip = "";

        string IHoverable.CursorGlyph => _cursorGlyph;

        string IHoverable.Tooltip => _toolkip;

        void IHoverable.OnPointerOut()
        {
        }

        void IHoverable.OnPointerOver()
        {
        }
    }
}