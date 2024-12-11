using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class RoomWorksFetcher : MonoBehaviour
{
    [Header("API Settings")]
    [SerializeField] private UserData userGameData;
    [SerializeField] public RoomController roomController;
    
    private string worksUrl;

#region Singleton pattern

    private static RoomWorksFetcher _instance;
    public static RoomWorksFetcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<RoomWorksFetcher>();
            }
            
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public void StartGetRoom(int roomId)
    {
        worksUrl = ConnectData.GetUserRoomUrl(roomId);
        FetchWorksFromRoom(roomId);
    }

    void FetchWorksFromRoom(int roomId)
    {
        StartCoroutine(GetWorksFromRoom());
    }

    private IEnumerator GetWorksFromRoom()
    {
        UnityWebRequest request = UnityWebRequest.Get(worksUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            RoomWorksResponse GetWorksFromRoomResponse = JsonConvert.DeserializeObject<RoomWorksResponse>(request.downloadHandler.text);
            roomController.SetWorksInRoom(GetWorksFromRoomResponse);
        }
        else
        {
            Debug.LogError($"Error fetching works from room: {request.error}");
        }
    }
}

[System.Serializable]
public class RoomWorksResponse
{
    public int RoomID;
    public List<Work> Works;
}