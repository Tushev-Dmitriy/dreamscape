using System;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene_Management
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _managerScene = default;
        [SerializeField] private GameSceneSO _menuToLoad = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private LoadEventChannelSO _menuLoadChannel = default;

        private void Start()
        {
            // Загрузка управляющей сцены (Manager Scene) как дополнительной
            SceneManager.LoadSceneAsync(_managerScene.SceneReference, LoadSceneMode.Additive).completed += LoadEventChannel;
        }

        private void LoadEventChannel(AsyncOperation obj)
        {
            // После загрузки Manager Scene вызываем событие для загрузки меню
            _menuLoadChannel.RaiseEvent(_menuToLoad, false, true);

            // Выгружаем сцену Initialization
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}