using Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scene_Management
{
    public class StartGame : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _hub;
        [SerializeField] private GameSceneSO _avatarCreation;
        [SerializeField] private bool _showLoadScreen = default;

        [Header("Broadcasting on")] 
        [SerializeField] private LoadEventChannelSO _loadHub = default;
        [SerializeField] private LoadEventChannelSO _loadAvatarCreation = default;

        [Header("Listening to")] 
        [SerializeField] private VoidEventChannelSO _startGameEvent = default;
        [SerializeField] private VoidEventChannelSO _startNewGameEvent = default;
        
        private void Start()
        {
            _startGameEvent.OnEventRaised += StartHub;
            _startNewGameEvent.OnEventRaised += StartNewGame;
        }

        private void OnDestroy()
        {
            _startGameEvent.OnEventRaised -= StartHub;
            _startNewGameEvent.OnEventRaised -= StartNewGame;
        }

        private void StartHub()
        {
            _loadHub.RaiseEvent(_hub, _showLoadScreen);
        }

        private void StartNewGame()
        {
            _loadAvatarCreation.RaiseEvent(_avatarCreation, _showLoadScreen);
        }
    }
}