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
            
            if (roomController != null)
            {
                roomController.SetWorksInRoom(GetWorksFromRoomResponse);
            }

            foreach (var work in GetWorksFromRoomResponse.Works)
            {

                switch (work.WorkType.ToLower())
                {
                    case "image":
                        for (int i = 0; i < userGameData.WorkSlot.ImagesSlot.Length; i++)
                        {
                            if (string.IsNullOrEmpty(userGameData.WorkSlot.ImagesSlot[i]))  // Проверка на пустой слот
                            {
                                if (work.WorkID != -1)
                                {
                                    userGameData.WorkSlot.ImagesSlot[i] = work.WorkID.ToString();
                                }  // Добавление работы в слот
                            }
                        }
                        break;
                    
                    case "music":
                        for (int i = 0; i < userGameData.WorkSlot.MusicSlot.Length; i++)
                        {
                            if (string.IsNullOrEmpty(userGameData.WorkSlot.MusicSlot[i]))
                            {
                                if (work.WorkID != -1)
                                {
                                    userGameData.WorkSlot.MusicSlot[i] = work.WorkID.ToString();
                                }
                            }
                        }
                        break;
                    
                    case "model":
                        for (int i = 0; i < userGameData.WorkSlot.ModelSlot.Length; i++)
                        {
                            if (string.IsNullOrEmpty(userGameData.WorkSlot.ModelSlot[i]))
                            {
                                if (work.WorkID != -1)
                                {
                                    userGameData.WorkSlot.ModelSlot[i] = work.WorkID.ToString();
                                }
                            }
                        }

                        break;
                }
            }
            
          //  setWorkSlotsChannel.RaiseEvent();

            if (roomController!= null)
            {
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