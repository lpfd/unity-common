namespace Leap.Forward
{
    public static class GameStateMachineExtensionMethods
    {
        public static void Enter<TState>(this IGameStateMachine stateMachine) where TState : IGameState
        {
            stateMachine.Enter(typeof(TState));
        }

        public static void Enter<TState, TPayload>(this IGameStateMachine stateMachine, TPayload payload) where TState : IGameState<TPayload>
        {
            stateMachine.Enter(typeof(TState), payload);
        }
    }
}