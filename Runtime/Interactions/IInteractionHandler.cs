using UnityEngine;
using UnityEngine.Events;

namespace Leap.Forward.Interactions
{
    public interface IInteractionHandler
    {
        void StartInteraction(GameObject gameObject);
        void StopInteraction(GameObject gameObject);
    }
}