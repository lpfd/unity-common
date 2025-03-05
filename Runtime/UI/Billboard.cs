using UnityEngine;

namespace Leap.Forward.UI
{
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}