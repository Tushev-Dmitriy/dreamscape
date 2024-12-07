using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class UserRegistration : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField loginField;
    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;
    public TMP_Text responseText;

    [Header("API Settings")]
    public ConnectData connectData;
    public UserData userGameData;

    private string registrationUrl;

    private void Awake()
    {
        registrationUrl = connectData.registrationUrl;
    }

    public void RegisterUser()
    {
        string login = loginField.text;
        string nickname = nicknameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
        {
            responseText.text = "All fields are required.";
            return;
        }

        UserRegistrationData userData = new UserRegistrationData
        {
            Login = login,
            Nickname = nickname,
            PasswordHash = password
        };

        StartCoroutine(SendRegistrationRequest(userData));
    }

    private IEnumerator SendRegistrationRequest(UserRegistrationData userData)
    {
        string jsonData = JsonUtility.ToJson(userData);

        UnityWebRequest request = new UnityWebRequest(registrationUrl, "POST");
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

            responseText.text = $"Registration successful: {response.Login}";
        }
        else
        {
            responseText.text = $"Error: {request.responseCode} - {request.error}\n{request.downloadHandler.text}";
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
