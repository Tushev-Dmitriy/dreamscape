using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;

public class RoomEventHandler : MonoBehaviourPunCallbacks
{
    public static Action onConnectedToMaster;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        onConnectedToMaster?.Invoke();

        Debug.Log("Connected to Server");

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("We are in the lobby");

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedLobby();

        Debug.Log("We are connected and in room");

    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        Debug.Log("Successfully left the room. Now creating a new room...");
    }

}
