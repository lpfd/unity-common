using System.ComponentModel;
using UnityEngine.Events;

namespace Leap.Forward.Composition
{
    /// <summary>
    /// Base container interface.
    /// </summary>
    public interface IContainer: INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// Is container initialized.
        /// If container is initialized all modules are resolved.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Triggered when the container has been successfully initialized.
        /// </summary>
        event UnityAction Initialized;
    }

    /// <summary>
    /// Container interface for particular type.
    /// </summary>
    /// <typeparam name="TContainer">Container type.</typeparam>
    public interface IContainer<TContainer>: IContainer where TContainer : class
    {
        /// <summary>
        /// Register module in container.
        /// Executed from module.
        /// </summary>
        /// <param name="module">Module to register.</param>
        void RegisterModule(IModule<TContainer> module);

        /// <summary>
        /// Unregister module from container.
        /// Executed from module.
        /// </summary>
        /// <param name="module">Module to unregister.</param>
        void UnregisterModule(IModule<TContainer> module);
    }
}
