using UnityEngine;
using VContainer;

namespace Leap.Forward.UI
{
    public class EnterGameState : MonoBehaviour
    {
        public GameStateReference _gameState;

        [Inject]
        public IGameStateMachine _gameStateMachine;

        public void HandleClick()
        {
            _gameStateMachine.Enter(_gameState.Type);
        }
    }
}