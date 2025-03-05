using System.Collections;
using UnityEngine;

namespace Leap.Forward
{
    public interface ISceneTransitionAnimator
    {
        object AnimateProgress(float deltaTime);
        Coroutine FadeIn();
        Coroutine FadeOut();
    }
}