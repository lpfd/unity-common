using System;
using UnityEngine;

namespace Leap.Forward
{
    public interface ISceneLoader
    {
        Coroutine Load(string name, Action onLoaded = null);
    }
}