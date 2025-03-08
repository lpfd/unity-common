using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Leap.Forward
{
    public interface IPrefabManager
    {
        GameObject Instantiate(GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent);
    }
    public class PrefabManager : IPrefabManager
    {
        IObjectResolver _objectResolver;
        public PrefabManager(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public GameObject Instantiate(GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
        {
            return _objectResolver.Instantiate(prefab);
        }
    }
}
