using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using ScriptsAPI;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserLogin : MonoBehaviour
{
    [Header("API Settings")]
    public UserData userGameData;
    
    [Header("Listening To")]
    [SerializeField] private UserLoginRequestSO _userLoginRequest = default;
    
    [Header("Broadcasting on")]
    [SerializeField] private BoolEventChannelSO _loginCorrectEvent = default;
    [SerializeField] private BoolEventChannelSO _serverErrorEvent = default;

    private string loginUrl;

    private void Awake()
    {
        loginUrl = ConnectData.LoginUrl;
        _userLoginRequest.OnEventRaised += Login;
    }

    private void OnDisable()
    {
        _userLoginRequest.OnEventRaised -= Login;
    }

    private void Login(string login, string password)
    {
        StartCoroutine(SendLoginRequest(login, password));
    }

    private IEnumerator SendLoginRequest(string login, string password)
    {
        var requestData = new UserLoginRequest { Login = login, Password = password };
        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            UserLoginResponse response = JsonUtility.FromJson<UserLoginResponse>(request.downloadHandler.text);

            userGameData.UserID = response.UserID;
            userGameData.Login = response.Login;
            userGameData.Nickname = response.Nickname;
            userGameData.RoomID = response.RoomID;

            _loginCorrectEvent.RaiseEvent(true);
        }
        else
        {
            if (request.downloadHandler.text.Contains("401"))
            {
                _loginCorrectEvent.RaiseEvent(false);
            }
            else
            {
                _serverErrorEvent.RaiseEvent(true);
            }
            
            Debug.Log($"Error: {request.responseCode} - {request.error}\n{request.downloadHandler.text}");
        }
    }
}

[System.Serializable]
public class UserLoginRequest
{
    public string Login;
    public string Password;
}