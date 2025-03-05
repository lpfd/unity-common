using UnityEngine;

namespace Leap.Forward.Transitions
{
    public class None : ISceneTransitionAnimator
    {
        public object AnimateProgress(float deltaTime)
        {
            return null;
        }

        public Coroutine FadeIn()
        {
            return null;
        }

        public Coroutine FadeOut()
        {
            return null;
        }
    }
}