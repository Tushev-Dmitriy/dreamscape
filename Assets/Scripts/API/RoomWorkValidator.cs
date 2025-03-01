using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        var array = workSlot.ImagesSlot.Concat(workSlot.MusicSlot).ToArray().Concat(workSlot.ModelSlot).ToArray();

        var intArray = new int[10];

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                intArray[i] = Convert.ToInt32(array[i]);
            }
            else
            {
                intArray[i] = -1;
            }
        }

        intArray[9] = -1;

        StartCoroutine(ValidateAndSaveCoroutine(roomId, intArray));
    }

    private IEnumerator ValidateAndSaveCoroutine(int roomId, int[] inputFields)
    {
        string addWorkUrl = ConnectData.PutWorkToSlot(roomId);
        
        var requestBody = new AddWorkRequest { works = inputFields };
        string jsonData = JsonUtility.ToJson(requestBody);
        Debug.Log(jsonData);

        UnityWebRequest addWorkRequest = new UnityWebRequest(addWorkUrl, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        addWorkRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        addWorkRequest.downloadHandler = new DownloadHandlerBuffer();
        addWorkRequest.SetRequestHeader("Content-Type", "application/json");

        yield return addWorkRequest.SendWebRequest();

        if (addWorkRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to add work in slot: {addWorkRequest.downloadHandler.text}");

            yield break;
        }
        
        Debug.Log("All slots validated and saved successfully!");
    }
}

[System.Serializable]
public class AddWorkRequest
{
    public int[] works;
}