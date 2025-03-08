using Leap.Forward.SaveFiles;
using Leap.Forward.Transitions;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Leap.Forward.LightInject
{
    public static class ExtensionMethods
    {
        public static IContainerBuilder WithGameStateMachine(this IContainerBuilder container)
        {
            container.Register<IGameStateMachine, GameStateMachine>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithSceneLoader(this IContainerBuilder container)
        {
            container.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithPrefabManager(this IContainerBuilder container)
        {
            container.Register<IPrefabManager, PrefabManager>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithSaveManager(this IContainerBuilder container)
        {
            container.Register<ISaveManager, SaveManager>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithUIManager(this IContainerBuilder container)
        {
            container.Register<IUIManager, UIManager>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithSceneTransitionAnimator(this IContainerBuilder container, ISceneTransitionAnimator transitionAnimator)
        {
            if (transitionAnimator != null)
                container.RegisterInstance<ISceneTransitionAnimator>(transitionAnimator);
            else
                container.RegisterInstance<ISceneTransitionAnimator>(new None());
            return container;
        }

        public static IContainerBuilder WithGameState<TState>(this IContainerBuilder container) where TState: IGameState
        {
            container.Register<TState, TState>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithGameState<TState, TPayload>(this IContainerBuilder container) where TState : IGameState<TPayload>
        {
            container.Register<TState, TState>(Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithFadeTransition(this IContainerBuilder container, CanvasGroup canvasGroup, float transitionTime = 0.5f)
        {

            container.Register<ISceneTransitionAnimator>(factory=>new FadeToBlack(factory.Resolve<ICoroutineRunner>(), canvasGroup, transitionTime), Lifetime.Singleton);
            return container;
        }

        public static IContainerBuilder WithEntryPoint(this IContainerBuilder container, Action entryPoint, Lifetime lifetime = Lifetime.Singleton)
        {
            container.RegisterEntryPoint<StartableCalback>(_ => new StartableCalback(entryPoint), lifetime);
            return container;
        }

        internal class StartableCalback : IStartable
        {
            private Action _callback;

            public StartableCalback(Action callback)
            {
                _callback = callback;
            }

            public void Start()
            {
                _callback?.Invoke();
            }
        }

    }
}