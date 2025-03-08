using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Leap.Forward
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISceneTransitionAnimator _transitionAnimator;

        public SceneLoader(ICoroutineRunner coroutineRunner, ISceneTransitionAnimator transitionAnimator = null)
        {
            _coroutineRunner = coroutineRunner;
            _transitionAnimator = transitionAnimator;
        }

        /// <summary>
        /// The name of the Scene that is currently active in the game or app.
        /// </summary>
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        public Coroutine Load(string name, Action onLoaded = null) =>
          _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded));

        private IEnumerator LoadScene(string nextScene, Action onLoaded = null)
        {
            if (string.IsNullOrEmpty(nextScene))
            {
                onLoaded?.Invoke();
                yield break;
            }

            if (SceneManager.GetActiveScene().name == nextScene)
            {
                onLoaded?.Invoke();
                yield break;
            }

            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                if (_transitionAnimator != null)
                {
                    yield return _transitionAnimator.FadeOut();
                }
            }

            if (BootstraperBase.Instance != null)
            {
                LifetimeScope.EnqueueParent(BootstraperBase.Instance);
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
            {
                if (_transitionAnimator != null)
                {
                    yield return _transitionAnimator.AnimateProgress(Time.deltaTime);
                }
                else
                {
                    yield return null;
                }
            }

            if (_transitionAnimator != null)
            {
                yield return _transitionAnimator.FadeIn();
            }

            onLoaded?.Invoke();
        }
    }
}