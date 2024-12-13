using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static Action onConnectedToMaster;
    public static Action onJoinedRoom;
    public static Action onLeftRoom;

    private string _roomForCreating;
    private string _sceneForLoad;

    [SerializeField] private string _hubSceneName;
    [SerializeField] private string _roomSceneName;
    [SerializeField] private string _hubRoomName;

    public void SwitchToRoom(string roomName)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving current room...");
            _roomForCreating = roomName;
            _sceneForLoad = _roomSceneName;
            PhotonNetwork.LeaveRoom(); 
        }
    }

    public void SwitchToHub()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving current room...");
            _roomForCreating = _hubRoomName;
            _sceneForLoad = _hubSceneName;
            PhotonNetwork.LeaveRoom();
        }
    }

    public void ConnectToHub()
    {
        Debug.Log("Connecting...");

        _roomForCreating = _hubRoomName;
        _sceneForLoad = _hubSceneName;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        onConnectedToMaster?.Invoke();

        PhotonNetwork.JoinOrCreateRoom(_roomForCreating, null, null);

        if (_sceneForLoad != SceneManager.GetActiveScene().name)
        {
            PhotonNetwork.LoadLevel(_sceneForLoad);
        }
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

}
