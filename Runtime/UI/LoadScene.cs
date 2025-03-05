using UnityEngine;
using VContainer;

namespace Leap.Forward.UI
{
    public class LoadScene : MonoBehaviour
    {
        public string _sceneName;

        [Inject]
        public ISceneLoader _sceneLoader;

        public void HandleClick()
        {
            _sceneLoader.Load(_sceneName);
        }
    }
}