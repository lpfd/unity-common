namespace Leap.Forward.Composition
{
    /// <summary>
    /// Base module interface.
    /// </summary>
    public interface IModule
    {
    }

    /// <summary>
    /// Module interface.
    /// </summary>
    /// <typeparam name="TContainer">Container type.</typeparam>
    public interface IModule<TContainer> : IModule where TContainer : class
    {
        /// <summary>
        /// Set current container to the module. As a response to the call the module should call RegisterModule in the container.
        /// Executed by container, should not be executed by any other scripts.
        /// </summary>
        /// <param name="container">Container the module gets attached to.</param>
        void SetContainer(TContainer container);
    }
}
