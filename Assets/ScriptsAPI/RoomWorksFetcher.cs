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

    public void StartGetRoom()
    {
        worksUrl = connectData.GetUserRoomUrl(userGameData.UserID);
        FetchWorksFromRoom(userGameData.RoomID);
    }

    void FetchWorksFromRoom(int roomId)
    {
        StartCoroutine(GetWorksFromRoom(roomId));
    }

    private IEnumerator GetWorksFromRoom(int roomId)
    {
        UnityWebRequest request = UnityWebRequest.Get(worksUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            RoomWorksResponse GetWorksFromRoomResponse = JsonConvert.DeserializeObject<RoomWorksResponse>(request.downloadHandler.text);
            roomController.roomWorksResponse = GetWorksFromRoomResponse;
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