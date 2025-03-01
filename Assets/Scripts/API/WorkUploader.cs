using System;
using System.Collections;
using System.IO;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using SimpleFileBrowser;

public class WorkUploader : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private VoidEventChannelSO successEventChannel;
    [SerializeField] private BoolEventChannelSO serverErrorEventChannel;
    [SerializeField] private WorkUploadHandlerSO _workUploadHandler;

    [Header("API Settings")]
    public UserData userGameData;

    private string addWorkUrl;
    private string filePath;

    private void OnEnable()
    {
        _workUploadHandler.OnEventRaised += SendWork;
    }

    private void OnDisable()
    {
        _workUploadHandler.OnEventRaised -= SendWork;
    }

    private void SendWork(UI.Work work)
    {
        int userId = userGameData.UserID;
        addWorkUrl = ConnectData.GetAddWorkUrl(userId);

        filePath = work.filePath;
        
        StartCoroutine(UploadWork(work));
    }

    private IEnumerator UploadWork(UI.Work work)
    {
        byte[] fileData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddField("work_title", work.title);
        form.AddField("work_type", work.workType);
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "application/octet-stream");

        UnityWebRequest request = UnityWebRequest.Post(addWorkUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Work successfully uploaded");
            WorkUploadResponse response = JsonUtility.FromJson<WorkUploadResponse>(request.downloadHandler.text);
            userGameData.WorksID.Add(response.WorkID);
            
            successEventChannel.RaiseEvent();
        }
        else
        {
            Debug.LogError($"Error: {request.responseCode} - {request.error}");
            
            serverErrorEventChannel.RaiseEvent(true);
        }
    }
}

[System.Serializable]
public class WorkUploadResponse
{
    public string message;
    public int WorkID;
}
