using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.IO;
using System;

public class UserWorksManager : MonoBehaviour
{
    [Header("API Settings")]
    public UserData userGameData;
    
    [SerializeField] private VoidEventChannelSO getWorkListEventChannel;
    [SerializeField] private VoidEventChannelSO setWorkListEventChannel;

    private string userWorksUrl;

    private void Start()
    {
        getWorkListEventChannel.OnEventRaised += FetchUserWorks;
    }

    private void OnDisable()
    {
        getWorkListEventChannel.OnEventRaised -= FetchUserWorks;
    }

    public void FetchUserWorks()
    {
        userWorksUrl = ConnectData.GetUserWorksUrl(userGameData.UserID);
        StartCoroutine(GetUserWorks());
    }

    private IEnumerator GetUserWorks()
    {
        UnityWebRequest request = UnityWebRequest.Get(userWorksUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                List<AllWork> works = JsonConvert.DeserializeObject<List<AllWork>>(request.downloadHandler.text);

                userGameData.WorksID.Clear();
                userGameData.AllWorks.Clear();

                foreach (AllWork work in works)
                {
                    userGameData.WorksID.Add(work.WorkID);
                    userGameData.AllWorks.Add(work);
                }
                
                setWorkListEventChannel.RaiseEvent();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing JSON: {ex.Message}");
            }
        }
        else
        {
            if (request.downloadHandler.text.Contains("404"))
            {
                Debug.LogError("Works not found");
            }
            else
            {
                Debug.LogError($"Error fetching user works: {request.responseCode} - {request.downloadHandler.text}");
            }
        }
    }

    public void DeleteWork(int workID)
    {
        StartCoroutine(DeleteWorkRequest(userGameData.UserID, workID));
    }

    private IEnumerator DeleteWorkRequest(int userId, int workId)
    {
        string deleteUrl = ConnectData.GetDeleteWorkUrl(userId, workId);

        UnityWebRequest request = UnityWebRequest.Delete(deleteUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Work {workId} successfully deleted");

            if (userGameData.WorksID.Contains(workId))
            {
                userGameData.WorksID.Remove(workId);
            }

            FetchUserWorks();
        }
        else
        {
            Debug.LogError($"Failed to delete work {workId}: {request.responseCode} - {request.error}");
        }
    }
}

[System.Serializable]
public class Work
{
    public int WorkID;
    public int UserID;
    public string WorkTitle;
    public string WorkType;
    public string WorkContent;
    public string DateAdded;
    public int LikesCount;
    public bool IsModerated;
}

[System.Serializable]
public class AllWork
{
    public int WorkID;
    public string WorkTitle;
    public string WorkType;
    public int LikesCount;
    public bool IsModerated;
}