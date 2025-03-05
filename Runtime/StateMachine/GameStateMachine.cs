using System;
using VContainer;

namespace Leap.Forward
{
    public class GameStateMachine: IGameStateMachine
    {
        private IGameStateBase _activeState;
        private IObjectResolver _serviceContainer;

        public IGameStateBase ActiveState => _activeState;

        public GameStateMachine(IObjectResolver serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }
        
        private void LeaveState()
        {
            if (_activeState != null)
            {
                _activeState.Exit();
                _activeState = null;
            }
        }

        public void Enter<TPayload>(Type stateType, TPayload payload)
        {
            if (stateType == null)
            {
                UnityEngine.Debug.LogError($"Can't enter undefined state");
                return;
            }
            var state = _serviceContainer.Resolve(stateType) as IGameState<TPayload>;
            if (state == null)
            {
                UnityEngine.Debug.LogError($"Can't resolve {stateType.Name} into IGameState<{typeof(TPayload).Name}>");
                return;
            }

            LeaveState();
            _activeState = state;
            state.Enter(payload);
        }

        public void Enter(Type stateType)
        {
            if (stateType == null)
            {
                UnityEngine.Debug.LogError($"Can't enter undefined state");
                return;
            }
            var state = _serviceContainer.Resolve(stateType) as IGameState;
            if (state == null)
            {
                UnityEngine.Debug.LogError($"Can't resolve {stateType.Name} into IGameState");
                return;
            }

            LeaveState();
            _activeState = state;
            state.Enter();
        }
    }
}
