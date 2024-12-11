using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class UserAvatarManager : MonoBehaviour
{
    [Header("API Settings")]
    public UserData userGameData;

    [Header("UI Elements")]
    public TMP_Dropdown eyeColorDropdown;
    public TMP_Dropdown hairStyleDropdown;
    public TMP_Dropdown skinColorDropdown;
    public TMP_Dropdown outfitDropdown;

    private string userAvatarUrl;

    private void Start()
    {
        int userId = userGameData.UserID;
        userAvatarUrl = ConnectData.GetUserAvatarUrl(userId);
    }

    public void FetchUserAvatarData()
    {
        StartCoroutine(GetUserAvatarData());
    }

    public void SetUserAvatarData()
    {
        int eyeColor = eyeColorDropdown.value + 1;
        int hairStyle = hairStyleDropdown.value + 1;
        int skinColor = skinColorDropdown.value + 1;
        int outfit = outfitDropdown.value + 1;

        AvatarData avatarData = new AvatarData
        {
            EyeColor = eyeColor,
            HairStyle = hairStyle,
            SkinColor = skinColor,
            Outfit = outfit
        };

        StartCoroutine(UpdateUserAvatarData(avatarData));
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
                SetDropdownValues(fetchedAvatar);

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

    private void SetDropdownValues(AvatarData avatarData)
    {
        eyeColorDropdown.value = avatarData.EyeColor - 1;
        hairStyleDropdown.value = avatarData.HairStyle - 1;
        skinColorDropdown.value = avatarData.SkinColor - 1;
        outfitDropdown.value = avatarData.Outfit - 1;
    }

    private IEnumerator UpdateUserAvatarData(AvatarData avatarData)
    {
        string jsonData = JsonConvert.SerializeObject(avatarData);

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
    public int EyeColor;
    public int HairStyle;
    public int SkinColor;
    public int Outfit;
}
