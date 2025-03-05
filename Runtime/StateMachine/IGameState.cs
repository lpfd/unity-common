namespace Leap.Forward
{
    public interface IGameState<T>: IGameStateBase
    {
        void Enter(T payload);
    }
    public interface IGameState : IGameStateBase
    {
        void Enter();
    }
}
