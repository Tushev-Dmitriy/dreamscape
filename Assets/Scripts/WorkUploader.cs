using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class WorkUploader : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField workTitleField;
    public TMP_InputField workTypeField;
    public TMP_Text selectedFileText;
    public Button uploadButton;
    public Button submitButton;

    [Header("API Settings")]
    public ConnectData connectData;
    public UserData userGameData;

    private string addWorkUrl;
    private string filePath;

    private void Start()
    {
        int userId = connectData.userGameData.UserID;
        addWorkUrl = connectData.GetAddWorkUrl(userId);
    }

    public void OpenFileExplorer()
    {
        string[] extensions = { "png", "jpg", "jpeg", "obj", "fbx" };
        filePath = UnityEditor.EditorUtility.OpenFilePanel("Select Work File", "", string.Join(",", extensions));

        if (!string.IsNullOrEmpty(filePath))
        {
            selectedFileText.text = $"Selected File: {Path.GetFileName(filePath)}";
        }
        else
        {
            selectedFileText.text = "No file selected";
        }
    }

    public void SendWork()
    {
        string workTitle = workTitleField.text;
        string workType = workTypeField.text;

        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(workTitle) || string.IsNullOrEmpty(workType))
        {
            Debug.LogError("All fields must be filled, and a file must be selected!");
            return;
        }

        StartCoroutine(UploadWork(workTitle, workType, filePath));
    }

    private IEnumerator UploadWork(string workTitle, string workType, string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);

        WWWForm form = new WWWForm();
        form.AddField("work_title", workTitle);
        form.AddField("work_type", workType);
        form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "application/octet-stream");

        UnityWebRequest request = UnityWebRequest.Post(addWorkUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Work successfully uploaded");
            WorkUploadResponse response = JsonUtility.FromJson<WorkUploadResponse>(request.downloadHandler.text);
            userGameData.WorksID.Add(response.WorkID);
        }
        else
        {
            Debug.LogError($"Error: {request.responseCode} - {request.error}");
        }
    }
}

[System.Serializable]
public class WorkUploadResponse
{
    public string message;
    public int WorkID;
}
