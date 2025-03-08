using UnityEngine;

namespace Leap.Forward.Composition
{
    /// <summary>
    /// Composition module base class.
    /// </summary>
    /// <typeparam name="TContainer">Conainer type.</typeparam>
    public class ModuleBase<TContainer> : MonoBehaviour, IModule<TContainer> where TContainer: class, IContainer<TContainer>
    {
        /// <summary>
        /// Container instance. Is set when module gets attached to container.
        /// </summary>
        public TContainer Container { get; private set; } = null;

        /// <summary>
        /// Called when the object becomes enabled and active, always after Awake (on the same object) and before any Start.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Container == null)
            {
                var container = gameObject.GetComponentInParent<TContainer>();

                if (container == null)
                {
                    UnityEngine.Debug.LogError($"{name} of {this.GetType().Name}: No contaier found in the parent game objects.");
                    return;
                }

                ((IModule<TContainer>)this).SetContainer(container);
            }

            if (!Container.IsInitialized)
            {
                Container.Initialized += HandleContainerInitialized;
            }
            else
            {
                SetupModule();
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            TearDownModule();
            ((IModule<TContainer>)this).SetContainer(null);
        }

        /// <summary>
        /// Method to setup module references. Executed when container is fully initialized.
        /// Executed from module's <see cref="OnEnable"/> method.
        /// </summary>
        public virtual void SetupModule()
        {
        }

        /// <summary>
        /// Method to release module references. Executed when module is disabled.
        /// </summary>
        public virtual void TearDownModule()
        {

        }

        /// <summary>
        /// Handle late container initialization.
        /// </summary>
        private void HandleContainerInitialized()
        {
            Container.Initialized -= HandleContainerInitialized;
            SetupModule();
        }

        /// <inheritdoc/>
        void IModule<TContainer>.SetContainer(TContainer container)
        {
            if (Container != container)
            {
                if (Container != null)
                {
                    Container.UnregisterModule(this);
                }
                Container = container;
                if (Container != null)
                {
                    Container.RegisterModule(this);
                }
            }
        }
    }
}
