using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Leap.Forward
{
    public class BootstraperBase : LifetimeScope, ICoroutineRunner
    {
        public static BootstraperBase Instance;

        [SerializeField]
        protected GameStateReference InitialGameState;

        public BootstraperBase()
        {
            Instance = this;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<ICoroutineRunner>(_=>this, Lifetime.Singleton);
        }

        protected override void Awake()
        {
            if (Container == null)
                this.Build();

            Instance = this;

            DontDestroyOnLoad(this);
        }

        internal void OverrideInitialStateWith(GameStateReference overrideState)
        {
            InitialGameState = overrideState;
        }
    }
}