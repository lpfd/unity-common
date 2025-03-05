using System.Collections;
using UnityEngine;

namespace Leap.Forward.Transitions
{
    public class FadeToBlack : ISceneTransitionAnimator
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly CanvasGroup _canvas;
        public readonly float _transitionTime;

        public FadeToBlack(ICoroutineRunner coroutineRunner, CanvasGroup canvas, float transitionTime = 0.5f)
        {
            _coroutineRunner = coroutineRunner;
            _canvas = canvas;
            _transitionTime = transitionTime;
        }

        public Coroutine FadeIn() => _coroutineRunner.StartCoroutine(FadeInImpl());

        private IEnumerator FadeInImpl()
        {
            if (_canvas == null || _transitionTime <= float.Epsilon)
            {
                if (_canvas != null)
                    _canvas.gameObject.SetActive(false);
                yield break;
            }

            _canvas.gameObject.SetActive(true);
            _canvas.alpha = 1.0f;
            while (_canvas.alpha > 0)
            {
                float deltaTime = Time.deltaTime / _transitionTime;
                _canvas.alpha = Mathf.Max(0, _canvas.alpha - deltaTime);
                yield return null;
            }
            _canvas.gameObject.SetActive(false);

        }

        public Coroutine FadeOut() => _coroutineRunner.StartCoroutine(FadeOutImpl());

        private IEnumerator FadeOutImpl()
        {
            if (_canvas == null || _transitionTime <= float.Epsilon)
            {
                if (_canvas != null)
                    _canvas.gameObject.SetActive(true);
                yield break;
            }
            _canvas.gameObject.SetActive(true);
            _canvas.alpha = 0.0f;
            while (_canvas.alpha < 1.0f)
            {
                float deltaTime = Time.deltaTime / _transitionTime;
                _canvas.alpha = Mathf.Min(1.0f, _canvas.alpha + deltaTime);
                yield return null;
            }
        }

        public object AnimateProgress(float deltaTime)
        {
            //progressIcon.Rotate(Vector3.forward, deltaTime*90);
            return null;
        }
    }
}
