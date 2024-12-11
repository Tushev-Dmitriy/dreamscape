using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using SimpleFileBrowser;

public class WorkUploader : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField workTitleField;
    public TMP_Dropdown workTypeDropdown;
    public TMP_Text selectedFileText;

    [Header("API Settings")]
    public UserData userGameData;

    private string addWorkUrl;
    private string filePath;

    private void Start()
    {
        int userId = userGameData.UserID;
        addWorkUrl = ConnectData.GetAddWorkUrl(userId);
    }

    public void OpenFileExplorer()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Images", ".png"), new FileBrowser.Filter("Music", ".mp3"), new FileBrowser.Filter("Models", ".fbx"));
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void SendWork()
    {
        string workTitle = workTitleField.text;
        int workTypeValue = workTypeDropdown.value;

        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(workTitle))
        {
            Debug.LogError("All fields must be filled, and a file must be selected!");
            return;
        }

        string workType = "";
        switch (workTypeValue)
        {
            case 0:
                workType = "Image";
                break;
            case 1:
                workType = "Music";
                break;
            case 2:
                workType = "Model";
                break;
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

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load");

        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
            OnFilesSelected(FileBrowser.Result);
    }

    void OnFilesSelected(string[] filePaths)
    {
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        filePath = filePaths[0];

        if (!string.IsNullOrEmpty(filePath))
        {
            selectedFileText.text = $"Selected File: {Path.GetFileName(filePath)}";
        }
        else
        {
            selectedFileText.text = "No file selected";
        }

    }
}

[System.Serializable]
public class WorkUploadResponse
{
    public string message;
    public int WorkID;
}
