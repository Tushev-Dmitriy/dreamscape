using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static Action onConnectedToMaster;
    public static Action onJoinedRoom;
    public static Action onLeftRoom;
    public static Action onTryDisconenct;
    public static Action onPlayerEnteredRoom;
    public static Action onPlayerLeftRoom;

    private string _roomForCreating;

    [SerializeField] private string _hubRoomName;

    private void Start()
    {
        SceneLoader.onHubLoaded += OnHubLoaded;
        SceneLoader.onRoomLoaded += OnRoomLoaded;
    }

    private void OnDestroy()
    {
        SceneLoader.onHubLoaded -= OnHubLoaded;
        SceneLoader.onRoomLoaded -= OnRoomLoaded;
    }

    public void Disconect()
    {
        onTryDisconenct?.Invoke();
        PhotonNetwork.Disconnect();
    }

    public void SetNextLoadRoom(string roomForCreating)
    {
        _roomForCreating = roomForCreating;
    }

    public void SwitchToRoom(string roomName)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving current room...");
            _roomForCreating = roomName;
            PhotonNetwork.LeaveRoom(); 
        }
    }

    public void SwitchToHub()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving current room...");
            _roomForCreating = _hubRoomName;
            PhotonNetwork.LeaveRoom(); 
        }
    }

    public void ConnectToServer()
    {
        Debug.Log("Connecting... ");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        onConnectedToMaster?.Invoke();

        PhotonNetwork.JoinOrCreateRoom(_roomForCreating, null, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedLobby();

        Debug.Log("We are connected and in room");

        onJoinedRoom?.Invoke();   

        Debug.LogError(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Successfully left the room. Now creating a new room...");
        onLeftRoom?.Invoke();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.LogError("новая комната");

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        onPlayerEnteredRoom?.Invoke();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);


        onPlayerLeftRoom?.Invoke();
    }

    private void OnHubLoaded()
    {
        //Debug.LogError(0);
        _roomForCreating = "hub";
        ConnectToServer();
    }

    private void OnRoomLoaded()
    {
        //Debug.LogError(0);
        ConnectToServer();
    }
}
