using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using ScriptsAPI;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

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

            StartCoroutine(GetUserAvatarData(userGameData.UserID));
            StartCoroutine(GetUserWorksFromRoom(userGameData.RoomID));
            
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

    private IEnumerator GetUserAvatarData(int userID)
    {
        UnityWebRequest requestAvatar = new UnityWebRequest(ConnectData.GetUserAvatarUrl(userID), "GET");
        requestAvatar.downloadHandler = new DownloadHandlerBuffer();
        requestAvatar.SetRequestHeader("Content-Type", "application/json");

        yield return requestAvatar.SendWebRequest();

        if (requestAvatar.result == UnityWebRequest.Result.Success)
        {
            AvatarData avatarResponse = JsonUtility.FromJson<AvatarData>(requestAvatar.downloadHandler.text);
            userGameData.AvatarData.HairStyle = avatarResponse.HairStyle;
            userGameData.AvatarData.Gender = avatarResponse.Gender;
            userGameData.AvatarData.OutfitTop = avatarResponse.OutfitTop;
            userGameData.AvatarData.OutfitDown = avatarResponse.OutfitDown;
        }
        else
        {
            if (requestAvatar.downloadHandler.text.Contains("401"))
            {
                _loginCorrectEvent.RaiseEvent(false);
            }
            else
            {
                _serverErrorEvent.RaiseEvent(true);
            }

            Debug.Log($"Error: {requestAvatar.responseCode} - {requestAvatar.error}\n{requestAvatar.downloadHandler.text}");
        }
    }
    
    private IEnumerator GetUserWorksFromRoom(int roomID)
    {
        UnityWebRequest requestWorks = new UnityWebRequest(ConnectData.GetUserWorksIdUrl(roomID), "GET");
        requestWorks.downloadHandler = new DownloadHandlerBuffer();
        requestWorks.SetRequestHeader("Content-Type", "application/json");

        yield return requestWorks.SendWebRequest();

        if (requestWorks.result == UnityWebRequest.Result.Success)
        {
            userGameData.ResetSlotsData();

            List<int> workIDs = JsonConvert.DeserializeObject<List<int>>(requestWorks.downloadHandler.text);

            userGameData.WorkSlot = new WorkSlot();

            for (int i = 0; i < workIDs.Count - 1; i++)
            {
                if (i < 3)
                {
                    if (workIDs[i] != 1)
                    {
                        userGameData.WorkSlot.ImagesSlot[i] = workIDs[i].ToString();
                    }
                }
                else if (i < 6)
                {
                    if (workIDs[i] != 1)
                    {
                        userGameData.WorkSlot.MusicSlot[i - 3] = workIDs[i].ToString();
                    }
                }
                else if (i < 9)
                {
                    if (workIDs[i] != 1)
                    {
                        userGameData.WorkSlot.ModelSlot[i - 6] = workIDs[i].ToString();
                    }
                }
            }
        }
        else
        {
            if (requestWorks.downloadHandler.text.Contains("404"))
            {
                _loginCorrectEvent.RaiseEvent(false);
            }
            else
            {
                _serverErrorEvent.RaiseEvent(true);
            }

            Debug.Log($"Error: {requestWorks.responseCode} - {requestWorks.error}\n{requestWorks.downloadHandler.text}");
        }
    }
}

[System.Serializable]
public class UserLoginRequest
{
    public string Login;
    public string Password;
}

