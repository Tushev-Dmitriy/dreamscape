using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UserLogin : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField loginField;
    public TMP_InputField passwordField;
    public TextMeshProUGUI responseText;

    [Header("API Settings")]
    public ConnectData connectData;
    public UserData userGameData;

    private string loginUrl;

    private void Awake()
    {
        loginUrl = connectData.loginUrl;
    }
    public void Login()
    {
        string login = loginField.text;
        string password = passwordField.text;

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

            responseText.text = $"Login successful: {response.Login}";

            connectData.regUserObj.SetActive(false);
        }
        else
        {
            responseText.text = $"Error: {request.responseCode} - {request.error}\n{request.downloadHandler.text}";
        }
    }
}

[System.Serializable]
public class UserLoginRequest
{
    public string Login;
    public string Password;
}