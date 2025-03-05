using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Leap.Forward.Composition
{
    /// <summary>
    /// Base class for container implementation.
    /// </summary>
    /// <typeparam name="TContainer">Container type. Should match type that inherits from the class.</typeparam>
    public class ContainerBase<TContainer> : MonoBehaviour, IContainer<TContainer> where TContainer:class
    {
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event UnityAction Initialized;

        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Modules registered in the container.
        /// </summary>
        private HashSet<IModule<TContainer>> _modules = new HashSet<IModule<TContainer>>();

        /// <summary>
        /// Find first module of a given type in the container.
        /// </summary>
        /// <typeparam name="TModule">Module type.</typeparam>
        /// <returns>Discovered module or null.</returns>
        public TModule GetModule<TModule>()
        {
            return GetModules<TModule>().FirstOrDefault();
        }

        /// <summary>
        /// Find all modules of a given type in the container.
        /// </summary>
        /// <typeparam name="TModule">Module type.</typeparam>
        /// <returns>Discovered modules.</returns>
        public IEnumerable<TModule> GetModules<TModule>()
        {
            if (!IsInitialized)
            {
                throw new Exception("Container is not yet initialized");
            }

            foreach (var module in _modules)
            {
                if (module is TModule targetModule)
                {
                    yield return targetModule;
                }
            }
        }

        /// <summary>
        /// First lifecycle function called when a new instance of an object is created. Always called before any Start functions.
        /// </summary>
        protected virtual void Awake()
        {
            DiscoverModules();
            IsInitialized = true;
            Initialized?.Invoke();
        }

        /// <summary>
        /// Called when the object becomes enabled and active, always after Awake (on the same object) and before any Start.
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// Called before the first frame update only if the script instance is enabled.
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {

        }

        /// <summary>
        /// Assign container properties according to module type.
        /// </summary>
        /// <param name="module">Attached module.</param>
        protected virtual void AttachModule(IModule<TContainer> module)
        {
        }

        /// <summary>
        /// Remove module from container properties according to module type.
        /// </summary>
        /// <param name="module">Detached module.</param>
        protected virtual void DetachModule(IModule<TContainer> module)
        {
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DiscoverModules()
        {
            var modules = gameObject.GetComponentsInChildren<IModule<TContainer>>();
            var container = this as TContainer;
            foreach (var module in modules)
            {
                module.SetContainer(container);
            }
        }

        /// <inheritdoc/>
        void IContainer<TContainer>.RegisterModule(IModule<TContainer> module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            if (!_modules.Add(module))
            {
                //Debug.LogError($"Module of type {module.GetType().Name} already registered");
                return;
            }

            AttachModule(module);
        }

        /// <inheritdoc/>
        void IContainer<TContainer>.UnregisterModule(IModule<TContainer> module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            if (!_modules.Remove(module))
            {
                //Debug.LogError($"Module of type {module.GetType().Name} is not registered");
                return;
            }
            DetachModule(module);
        }
    }
}
