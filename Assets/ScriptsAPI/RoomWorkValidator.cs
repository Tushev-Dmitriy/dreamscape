using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class RoomWorkValidator : MonoBehaviour
{
    public TMP_InputField[] imageFields;
    public TMP_InputField[] musicFields;
    public TMP_InputField[] modelFields; 

    public ConnectData connectData;
    public UserData userData;

    public void ValidateAndSaveRoomWorks()
    {
        int roomId = userData.RoomID;
        StartCoroutine(ValidateAndSaveCoroutine(roomId));
    }

    private IEnumerator ValidateAndSaveCoroutine(int roomId)
    {
        List<(int slot, TMP_InputField field, string type)> slots = new List<(int, TMP_InputField, string)>
        {
            (1, imageFields[0], "image"), (2, imageFields[1], "image"), (3, imageFields[2], "image"),
            (4, musicFields[0], "music"), (5, musicFields[1], "music"), (6, musicFields[2], "music"),
            (7, modelFields[0], "model"), (8, modelFields[1], "model"), (9, modelFields[2], "model")
        };

        foreach (var (slot, field, type) in slots)
        {
            string workIdText = field.text.Trim();

            if (string.IsNullOrEmpty(workIdText))
            {
                continue;
            }

            if (!int.TryParse(workIdText, out int workId))
            {
                Debug.LogError($"Invalid ID in slot {slot}");
                yield break;
            }

            string validationUrl = connectData.GetValidationUrl(workId, type);
            UnityWebRequest validationRequest = UnityWebRequest.Get(validationUrl);
            yield return validationRequest.SendWebRequest();

            if (validationRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Validation failed for slot {slot}: {validationRequest.downloadHandler.text}");
                yield break;
            }

            string addWorkUrl = connectData.GetAddWorkUrl(roomId, slot);
            var requestBody = new AddWorkRequest { work_id = workId, user_id = userData.UserID };
            string jsonData = JsonUtility.ToJson(requestBody);

            UnityWebRequest addWorkRequest = new UnityWebRequest(addWorkUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            addWorkRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            addWorkRequest.downloadHandler = new DownloadHandlerBuffer();
            addWorkRequest.SetRequestHeader("Content-Type", "application/json");

            yield return addWorkRequest.SendWebRequest();

            if (addWorkRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to add work in slot {slot}: {addWorkRequest.downloadHandler.text}");
                yield break;
            }
        }

        Debug.Log("All slots validated and saved successfully!");
    }
}

[System.Serializable]
public class AddWorkRequest
{
    public int work_id;
    public int user_id;
}