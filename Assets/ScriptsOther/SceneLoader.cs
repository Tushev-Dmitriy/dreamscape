using System;
using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static Action onHubLoaded;
    public static Action onRoomLoaded;

    [Header("Listening To")]
    [SerializeField] private LoadEventChannelSO _loadHub;
    [SerializeField] private LoadEventChannelSO _loadAvatarCreation;
    [SerializeField] private LoadEventChannelSO _loadRoom;
    [SerializeField] private LoadEventChannelSO _loadMenu;
    [SerializeField] private LoadEventChannelSO _backToHub;
    
    [SerializeField] private BoolEventChannelSO _onRoomLoadedEvent;
    [SerializeField] private BoolEventChannelSO _onHubLoadedEvent;

    [Header("Broadcasting on")]
    [SerializeField] private BoolEventChannelSO _toggleLoadingScreen;
    [SerializeField] private BoolEventChannelSO onRoomLoadedChannel;
    [SerializeField] private FadeChannelSO _fadeRequestChannel = default;
    
    private GameSceneSO _sceneToLoad;
    private GameSceneSO _currentlyLoadedScene;
    private bool _showLoadingScreen;
    
    private float _fadeDuration = .5f;
    private bool _isLoading = false;
    
    private static SceneLoader _instance;

    public static SceneLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneLoader>();
            }
            
            return _instance;
        }
    }
    
    private void OnEnable()
    {
        _loadHub.OnLoadingRequested += LoadScene;
        _loadAvatarCreation.OnLoadingRequested += LoadScene;
        _loadMenu.OnLoadingRequested += LoadScene;
        _loadRoom.OnLoadingRequested += LoadScene;

        _backToHub.OnLoadingRequested += BackToHub;
    }

    private void OnDisable()
    {
        _loadHub.OnLoadingRequested -= LoadScene;
        _loadAvatarCreation.OnLoadingRequested -= LoadScene;
        _loadMenu.OnLoadingRequested -= LoadScene;
        _loadRoom.OnLoadingRequested -= LoadScene;
        
        _backToHub.OnLoadingRequested -= BackToHub;

    }

    private void BackToHub(GameSceneSO menuToLoad, bool showLoadingScreen, bool fadeScreen)
    {
        if (_currentlyLoadedScene != null)
        {
            if (_currentlyLoadedScene.SceneType == GameSceneSO.GameSceneType.Hub) return;

        }
        
        LoadScene(menuToLoad, showLoadingScreen, fadeScreen);
    }
    
    private void LoadScene(GameSceneSO menuToLoad, bool showLoadingScreen, bool fadeScreen)
    {
        if (_isLoading) return;

        _sceneToLoad = menuToLoad;
        _showLoadingScreen = showLoadingScreen;
        _isLoading = true;

        StartCoroutine(UnloadPreviousScene());
    }
    
    private IEnumerator UnloadPreviousScene()
    {
        _fadeRequestChannel.FadeOut(_fadeDuration);

        yield return new WaitForSeconds(_fadeDuration);

        if (_currentlyLoadedScene != null)
        {
            if (!string.IsNullOrEmpty(_currentlyLoadedScene.SceneReference))
            {
                SceneManager.UnloadSceneAsync(_currentlyLoadedScene.SceneReference);
            }
#if UNITY_EDITOR
            else
            {
                SceneManager.UnloadSceneAsync(_currentlyLoadedScene.SceneReference);
            }
#endif
        }

        LoadNewScene();
    }
    
    private void IsHubLoading()
    {
        if (_sceneToLoad.SceneType == GameSceneSO.GameSceneType.Hub)
        {
            _onHubLoadedEvent.RaiseEvent(true);
            onHubLoaded?.Invoke();
        }
    }

    private void IsRoomLoading()
    {
        if (_sceneToLoad.SceneType == GameSceneSO.GameSceneType.Room)
        {
            _onRoomLoadedEvent.RaiseEvent(true);
            onRoomLoaded?.Invoke();
        }
    }
    
    private void LoadNewScene()
    {
        if (_showLoadingScreen)
        {
            _toggleLoadingScreen.RaiseEvent(true);
        }

        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(_sceneToLoad.SceneReference, LoadSceneMode.Additive);
        loadingOperation.completed += OnNewSceneLoaded;
    }
    
    
    private void OnNewSceneLoaded(AsyncOperation obj)
    {
        _currentlyLoadedScene = _sceneToLoad;

        IsHubLoading();
        IsRoomLoading();

        Scene loadedScene = SceneManager.GetSceneByName(_sceneToLoad.SceneReference);
        SceneManager.SetActiveScene(loadedScene);

       if (_sceneToLoad.SceneType == GameSceneSO.GameSceneType.Room)
        {
            StartCoroutine(WaitForServerResponse());
        }
        else
        {
            FinalizeSceneLoad();
        }
    }

    private IEnumerator WaitForServerResponse()
    {
        bool isResponseReceived = false;

        // Подписка на событие окончания загрузки данных
        onRoomLoadedChannel.OnEventRaised += (response) =>
        {
            isResponseReceived = response;
        };
        
        // Ждем, пока ответ не будет получен
        while (!isResponseReceived)
        {
            yield return null;
        }

        FinalizeSceneLoad();
        
    }

    private void FinalizeSceneLoad()
    {
        _isLoading = false;

        if (_showLoadingScreen)
        {
            _toggleLoadingScreen.RaiseEvent(false);
        }

        _fadeRequestChannel.FadeIn(_fadeDuration);
    }
}
