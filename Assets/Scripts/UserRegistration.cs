using System;
using System.Collections;
using Events;
using ScriptsAPI;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UserRegistration : MonoBehaviour
{
    [Header("Listening To")] [SerializeField]
    private UserLoginRequestSO _userRegisterRequest = default;

    [Header("Broadcasting on")] [SerializeField]
    private BoolEventChannelSO _serverErrorEvent = default;

    [Header("API Settings")] public UserData userGameData;

    private void Awake()
    {
        _userRegisterRequest.OnEventRaised += RegisterUser;
    }

    private void OnDisable()
    {
        _userRegisterRequest.OnEventRaised -= RegisterUser;
    }

    private void RegisterUser(string login, string password)
    {
        Debug.Log(login + "\n" + password);
        UserRegistrationData userData = new UserRegistrationData
        {
            Login = login,
            Nickname = login,
            PasswordHash = password
        };

        StartCoroutine(SendRegistrationRequest(userData));
    }

    private IEnumerator SendRegistrationRequest(UserRegistrationData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);

        UnityWebRequest request = new UnityWebRequest(ConnectData.RegistrationUrl, "POST");
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
            
            _serverErrorEvent.RaiseEvent(false);
        }
        else
        {
            _serverErrorEvent.RaiseEvent(true);
            
            Debug.Log($"Error: {request.responseCode} - {request.error}\n{request.downloadHandler.text}");
        }
    }
}

[System.Serializable]
public class UserRegistrationData
{
    public string Login;
    public string Nickname;
    public string PasswordHash;
}