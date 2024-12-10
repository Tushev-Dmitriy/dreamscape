using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadMainScene();
    }

    public void SwitchScene(bool isRoom, int roomId)
    {
        if (isRoom)
        {
            SceneManager.UnloadSceneAsync("MainScene");
            LoadRoomScene(roomId);
            SetPlayerPosition(new Vector3(0, 1, 0));
        }
        else
        {
            SceneManager.UnloadSceneAsync("RoomScene");
            SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Additive);
            SetPlayerPosition(new Vector3(0, 1, 0));
        }
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void LoadRoomScene(int roomId)
    {
        SceneManager.LoadSceneAsync("RoomScene", LoadSceneMode.Additive);

        currentRoomIdEvent.RaiseEvent(roomId);

        SceneManager.sceneLoaded += OnRoomSceneLoaded;
    }

    void OnRoomSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject mainSceneObject = GameObject.FindGameObjectWithTag("Portal");
        mainSceneObject.GetComponent<RoomTransition>().sceneLoader = this;
        GameObject roomFetcherSceneObject = GameObject.FindGameObjectWithTag("API");
        GameObject roomControllerSceneObject = GameObject.FindGameObjectWithTag("RoomController");
        RoomController tempRoom = roomControllerSceneObject.GetComponent<RoomController>();
        roomFetcherSceneObject.GetComponent<RoomWorksFetcher>().roomController = tempRoom;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            GameObject[] mainSceneObject = GameObject.FindGameObjectsWithTag("Portal");
            for (int i = 0; i < mainSceneObject.Length; i++)
            {
                mainSceneObject[i].GetComponent<RoomTransition>().sceneLoader = this;
            }

            SetPlayerPosition(new Vector3(0, 1, 0));
        }
    }

    void SetPlayerPosition(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<MoveController>().SetPlayerPos();
    }
}
