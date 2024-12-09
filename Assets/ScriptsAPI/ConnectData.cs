using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectData : MonoBehaviour
{
    [Header("URLS")]
    private static string host = "http://77.91.78.231:8081/";
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
        registrationUrl = $"{host}auth/register";
        loginUrl = $"{host}auth/login";
        userWorksUrl = $"{host}works/";
        addWorkBaseUrl = $"{host}works/";
    }

    public string GetAddWorkUrl(int userId)
    {
        return $"{addWorkBaseUrl}{userId}/add";
    }

    public string GetUserWorksUrl(int userId)
    {
        return $"{userWorksUrl}{userId}";
    }

    public string GetDeleteWorkUrl(int userId, int workId)
    {
        return $"{host}works/{userId}/{workId}/delete";
    }

    public string GetUserAvatarUrl(int userId)
    {
        return $"{host}user/{userId}/avatar";
    }

    public string GetUserRoomUrl(int ñurrentRoomID)
    {
        return $"{host}room/{ñurrentRoomID}/works";
    }

    public string GetLikeWorkUrl(int workId)
    {
        return $"{host}works/{workId}/like";
    }

    public string GetValidationUrl(int workId, string type)
    {
        return $"{host}works/{workId}/validate/{type}";
    }

    public string GetAddWorkUrl(int roomId, int slot)
    {
        return $"{host}room/{roomId}/{slot}/add_work/";
    }

}