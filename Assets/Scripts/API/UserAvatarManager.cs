using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Serialization;

public class UserAvatarManager : MonoBehaviour
{
    [Header("API Settings")]
    public UserData userGameData;

    [SerializeField] private VoidEventChannelSO onSaveAvatarData;

    private string userAvatarUrl;

    private void OnEnable()
    {
        onSaveAvatarData.OnEventRaised += SetUserAvatarData;
    }

    private void OnDisable()
    {
        onSaveAvatarData.OnEventRaised -= SetUserAvatarData;
    }

    public void FetchUserAvatarData()
    {
        int userId = userGameData.UserID;
        userAvatarUrl = ConnectData.GetUserAvatarUrl(userId);
        
    }

    public void SetUserAvatarData()
    {
        int userId = userGameData.UserID;
        userAvatarUrl = ConnectData.GetUserAvatarUrl(userId);
        
        int gender = userGameData.AvatarData.Gender;
        int hairStyle = userGameData.AvatarData.HairStyle;
        int outfitTop = userGameData.AvatarData.OutfitTop;
        int outfitDown = userGameData.AvatarData.OutfitDown;

        AvatarData avatarData = new AvatarData
        {
            Gender = gender,
            HairStyle = hairStyle,
            OutfitTop = outfitTop,
            OutfitDown = outfitDown
        };

        StartCoroutine(UpdateUserAvatarData());
    }

    private IEnumerator GetUserAvatarData()
    {
        UnityWebRequest request = UnityWebRequest.Get(userAvatarUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                AvatarData fetchedAvatar = JsonConvert.DeserializeObject<AvatarData>(request.downloadHandler.text);

                Debug.Log($"Avatar data fetched: {JsonConvert.SerializeObject(fetchedAvatar)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Error fetching avatar data: {request.responseCode} - {request.downloadHandler.text}");
        }
    }

    private IEnumerator UpdateUserAvatarData()
    {
        string jsonData = JsonConvert.SerializeObject(userGameData.AvatarData);

        UnityWebRequest request = new UnityWebRequest(userAvatarUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Avatar data successfully updated: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"Error updating avatar data: {request.responseCode} - {request.error}");
        }
    }
}

[System.Serializable]
public class AvatarData
{
    public int HairStyle;
    public int Gender;
    public int OutfitTop;
    public int OutfitDown;
}
