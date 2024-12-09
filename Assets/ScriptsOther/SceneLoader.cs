using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

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

    public void SwitchScene(bool isRoom)
    {
        if (isRoom)
        {
            SceneManager.UnloadSceneAsync("MainScene");
            LoadRoomScene();
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

    void LoadRoomScene()
    {
        SceneManager.LoadSceneAsync("RoomScene", LoadSceneMode.Additive);

        SceneManager.sceneLoaded += OnRoomSceneLoaded;
    }

    void OnRoomSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject mainSceneObject = GameObject.FindGameObjectWithTag("Portal");
        mainSceneObject.GetComponent<RoomTransition>().sceneLoader = this;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            GameObject mainSceneObject = GameObject.FindGameObjectWithTag("Portal");
            mainSceneObject.GetComponent<RoomTransition>().sceneLoader = this;

            SetPlayerPosition(new Vector3(0, 1, 0));
        }
    }

    void SetPlayerPosition(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<MoveController>().SetPlayerPos();
    }
}
