using UnityEngine;

namespace Leap.Forward
{
    public interface IUnityComponent
    {
        /// <summary>
        /// Component's game object.
        /// </summary>
        GameObject GameObject { get; }
    }
}