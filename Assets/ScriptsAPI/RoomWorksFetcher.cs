using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class RoomWorksFetcher : MonoBehaviour
{
    [Header("API Settings")]
    public ConnectData connectData;
    public UserData userGameData;
    public RoomController roomController;

    private string worksUrl;

    public void StartGetRoom(int currentRoomID)
    {
        worksUrl = connectData.GetUserRoomUrl(currentRoomID);
        FetchWorksFromRoom(currentRoomID);
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