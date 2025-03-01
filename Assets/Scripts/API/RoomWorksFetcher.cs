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

    [SerializeField] private BoolEventChannelSO onRoomLoadedChannel;
    [SerializeField] private VoidEventChannelSO getRoomWorkEventChannel;
   // [SerializeField] private VoidEventChannelSO setWorkSlotsChannel;
    [SerializeField] private VoidEventChannelSO listEnabledChannel;
    
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

    private void Start()
    {
        getRoomWorkEventChannel.OnEventRaised += GetRoomWorks;

       // listEnabledChannel.OnEventRaised += () => { setWorkSlotsChannel.RaiseEvent(); };
    }

    private void OnDisable()
    {
        getRoomWorkEventChannel.OnEventRaised -= GetRoomWorks;
        
        //listEnabledChannel.OnEventRaised -= () => { setWorkSlotsChannel.RaiseEvent(); };
    }

    private void GetRoomWorks()
    {
        worksUrl = ConnectData.GetUserRoomUrl(userGameData.RoomID);

        FetchWorksFromRoom();
        
    }
    
    public void StartGetRoom(int roomId)
    {
        worksUrl = ConnectData.GetUserRoomUrl(roomId);
        FetchWorksFromRoom();
    }

    void FetchWorksFromRoom()
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
            
            Debug.Log(GetWorksFromRoomResponse.RoomID);
            
            if (roomController != null)
            {
                roomController.SetWorksInRoom(GetWorksFromRoomResponse);
            }
            
            
          //  setWorkSlotsChannel.RaiseEvent();

            if (roomController!= null)
            {
                Debug.Log("room loaded");

                onRoomLoadedChannel.RaiseEvent(true);
            }
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