using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Leap.Forward
{
    public class EnsureBootstraper : LifetimeScope
    {
        public BootstraperBase _bootstraperPrefab;

        [SerializeField]
        public GameStateReference _overrideGameState;

        protected override void Awake()
        {
            autoRun = false;
            var bootstraper = FindOrCreateBootstraper();

            base.Awake();

            //var lifetimeScope = GetComponent<LifetimeScope>();
            //lifetimeScope.parentReference = ParentReference.Create<>(bootstraper.GetType()) { Object = bootstraper};
            //lifetimeScope.Build();

            Build();
        }

        protected override LifetimeScope FindParent()
        {
            Debug.Log("FindParent executed");
            return FindOrCreateBootstraper();
        }

        private LifetimeScope FindOrCreateBootstraper()
        {
            var bootstraper = FindAnyObjectByType<BootstraperBase>();
            if (bootstraper == null)
            {
                bootstraper = UnityEngine.Object.Instantiate(_bootstraperPrefab);
                bootstraper.OverrideInitialStateWith(_overrideGameState);
            }

            return bootstraper;
        }

        internal void PopulateComponentList()
        {
            var list = new List<GameObject>();

            var visitedComponents = new Dictionary<Type, bool>();

            foreach (var gameObject in EnumerateAllGameObjects())
            {
                if (gameObject.GetComponents<MonoBehaviour>().Any(_ => HasInjectedFieldsOrMethods(_.GetType(), visitedComponents)))
                {
                    list.Add(gameObject);
                }
            }

            if (list.Count > 0)
            {
                autoInjectGameObjects = list;
            }
            else if (autoInjectGameObjects.Count > 0)
            {
                autoInjectGameObjects = list;
            }
        }

        private bool HasInjectedFieldsOrMethods(Type componentType, Dictionary<System.Type, bool> visitedComponents)
        {
            if (componentType == null || componentType == typeof(MonoBehaviour))
                return false;

            if (visitedComponents.TryGetValue(componentType, out var cachedValue))
                return cachedValue;

            cachedValue = HasInjectedFieldsOrMethods(componentType.BaseType, visitedComponents) || HasInjectedFieldsOrMethods(componentType);
            visitedComponents.Add(componentType, cachedValue);
            return cachedValue;
        }

        private bool HasInjectedFieldsOrMethods(Type componentType)
        {
            foreach (var field in componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.GetCustomAttribute<VContainer.InjectAttribute>() != null)
                {
                    return true;
                }
            }

            foreach (var field in componentType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.GetCustomAttribute<VContainer.InjectAttribute>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        private List<GameObject> EnumerateAllGameObjects()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                EnumerateGameObjectsRecursively(go, gameObjects);
            }
            return gameObjects;
        }

        private void EnumerateGameObjectsRecursively(GameObject gameObject, List<GameObject> gameObjects)
        {
            gameObjects.Add(gameObject);
            foreach (Transform child in gameObject.transform)
            {
                EnumerateGameObjectsRecursively(child.gameObject, gameObjects);
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(EnsureBootstraper))]
    public class EnsureBootstraperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector elements
            DrawDefaultInspector();

            // Add a space for better layout
            GUILayout.Space(10);

            // Create a button and define its behavior
            if (GUILayout.Button("Populate Auto Inject Game Objects"))
            {
                // Reference to the target component
                EnsureBootstraper myComponent = (EnsureBootstraper)target;

                Undo.RecordObject(this, "Modified Auto Inject Game Objects");
                EditorUtility.SetDirty(myComponent);

                myComponent.PopulateComponentList();

                serializedObject.ApplyModifiedProperties();
            }
        }
    }
#endif
}