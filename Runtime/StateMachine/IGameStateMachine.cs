using System;

namespace Leap.Forward
{
    public interface IGameStateMachine
    {
        IGameStateBase ActiveState { get; }

        void Enter(Type stateType);

        void Enter<TPayload>(Type stateType, TPayload payload);
    }
}
