using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectData : MonoBehaviour
{
    [Header("URLS")]
    private static string host = "http://77.91.78.231:8080/";
    public string registrationUrl;
    public string loginUrl;
    public string userWorksUrl;
    public string addWorkBaseUrl;

    [Header("Obj in scene")]
    public GameObject regUserObj;
    public GameObject loginUserObj;

    [Header("Scripts")]
    public UserData userGameData;

    private void Awake()
    {
        registrationUrl = $"{host}auth/register/";
        loginUrl = $"{host}auth/login/";
        userWorksUrl = $"{host}works/";
        addWorkBaseUrl = $"{host}works/";
    }

    private void Start()
    {
        Debug.Log(userWorksUrl);
    }

    public string GetAddWorkUrl(int userId)
    {
        return $"{addWorkBaseUrl}{userId}/add/";
    }

    public string GetUserWorksUrl(int userId)
    {
        return $"{userWorksUrl}{userId}/";
    }

    public string GetDeleteWorkUrl(int userId, int workId)
    {
        return $"{host}works/{userId}/{workId}/delete/";
    }

    public string GetUserAvatarUrl(int userId)
    {
        return $"{host}user/{userId}/avatar";
    }

    public string GetUserRoomUrl(int userId)
    {
        return $"{host}room/{userId}/works/";
    }
}