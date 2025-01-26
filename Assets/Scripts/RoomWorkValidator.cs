using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class RoomWorkValidator : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO setWorksInSlotsEvent;
    [SerializeField] private UserData userData;

    private void OnEnable()
    {
        setWorksInSlotsEvent.OnEventRaised += ValidateAndSaveRoomWorks;
    }

    private void OnDisable()
    {
        setWorksInSlotsEvent.OnEventRaised -= ValidateAndSaveRoomWorks;
    }

    public void ValidateAndSaveRoomWorks()
    {
        var workSlot = userData.WorkSlot;
        int roomId = userData.RoomID;
        StartCoroutine(ValidateAndSaveCoroutine(roomId, workSlot.ImagesSlot, workSlot.MusicSlot, workSlot.ModelSlot));
    }

    private IEnumerator ValidateAndSaveCoroutine(int roomId, string[] imageFields, string[] musicFields,
        string[] modelFields)
    {
        List<(int slot, string field, string type)> slots = new List<(int, string, string)>
        {
            (1, imageFields[0], "image"), (2, imageFields[1], "image"), (3, imageFields[2], "image"),
            (4, musicFields[0], "music"), (5, musicFields[1], "music"), (6, musicFields[2], "music"),
            (7, modelFields[0], "model"), (8, modelFields[1], "model"), (9, modelFields[2], "model")
        };

        foreach (var (slot, field, type) in slots)
        {
            string workIdText;
            if (field != null)
            {
                Debug.Log(field.Trim());
                workIdText = field.Trim();
            }
            else
            {
                workIdText = null;
            }

            if (string.IsNullOrEmpty(workIdText))
            {
                continue;
            }

            if (!int.TryParse(workIdText, out int workId))
            {
                Debug.LogError($"Invalid ID in slot {slot}");
                yield break;
            }

            string validationUrl = ConnectData.GetValidationUrl(workId, type);
            UnityWebRequest validationRequest = UnityWebRequest.Get(validationUrl);
            yield return validationRequest.SendWebRequest();

            if (validationRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Validation failed for slot {slot}: {validationRequest.downloadHandler.text}");
                yield break;
            }

            string addWorkUrl = ConnectData.GetAddWorkUrl(roomId, slot);
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

                //userData.ResetSlotsData();
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