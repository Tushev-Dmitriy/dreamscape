using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectData : MonoBehaviour
{
    [Header("URLS")]
    private static string host = "http://localhost:8000/";
    public string registrationUrl = $"{host}auth/register/";
    public string loginUrl = $"{host}auth/login/";
    public string userWorksUrl = $"{host}works/";
    public string addWorkBaseUrl = $"{host}works/";

    [Header("Obj in scene")]
    public GameObject regUserObj;
    public GameObject loginUserObj;

    [Header("Scripts")]
    public UserData userGameData;

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