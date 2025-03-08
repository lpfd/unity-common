using UnityEngine;
using VContainer;

namespace Leap.Forward
{
    public class ToggleGameObject : MonoBehaviour
    {
        public GameObject[] _enable;
        public GameObject[] _disable;
        public GameObject[] _toggle;

        public void HandleClick()
        {
            foreach (var go in _enable)
                go.SetActive(true);
            foreach (var go in _disable)
                go.SetActive(false);
            foreach (var go in _toggle)
                go.SetActive(!go.activeSelf);
        }
    }
}
