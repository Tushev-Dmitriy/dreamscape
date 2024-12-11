using System;
using Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Scene_Management
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _managerScene = default;
        [SerializeField] private GameSceneSO _menuToLoad = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference _menuLoadChannel = default;

        private void Start()
        {
            _managerScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }
        
        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
            _menuLoadChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += LoadMainMenu;
        }

        private void LoadMainMenu(AsyncOperationHandle<LoadEventChannelSO> obj)
        {
            obj.Result.RaiseEvent(_menuToLoad, false, true);

            SceneManager.UnloadSceneAsync(0);
        }
    }
}