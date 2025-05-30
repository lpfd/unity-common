using System.Collections.Generic;
using UnityEngine;

namespace Leap.Forward.Interactions
{
    public interface IInteractable
    {
        void StartInteraction(GameObject gameObject);
        void StopInteraction(GameObject gameObject);
    }
}