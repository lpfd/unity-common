using System;
using UnityEngine;

namespace Leap.Forward
{
    public interface ISceneLoader
    {
        string CurrentSceneName { get; }

        Coroutine Load(string name, Action onLoaded = null);
    }
}