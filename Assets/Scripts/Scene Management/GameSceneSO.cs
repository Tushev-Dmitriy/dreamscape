using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneSO", menuName = "SceneSO")]
public class GameSceneSO : ScriptableObject
{
    public GameSceneType SceneType;
    public string SceneReference;
    public enum GameSceneType
    {
        Menu,
        Manager,
        AvatarCreation,
        Hub,
        Room
    }
}
