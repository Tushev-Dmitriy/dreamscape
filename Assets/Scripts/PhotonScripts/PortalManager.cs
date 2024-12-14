using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> portals;
    [SerializeField] private UserData _userData;

    private void Start()
    {
        RoomManager.onJoinedRoom += OpenPortals;
        RoomManager.onPlayerEnteredRoom += OpenPortals;
        RoomManager.onPlayerLeftRoom += OpenPortals;
    }

    private void OnDestroy()
    {
        RoomManager.onJoinedRoom -= OpenPortals;
        RoomManager.onPlayerLeftRoom -= OpenPortals;
        RoomManager.onPlayerEnteredRoom -= OpenPortals;
    }

    private void OpenPortals()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (portals.Count < playerCount)
            return;

        for (int i = 0; i < playerCount; i++)
        {
            portals[i].SetActive(true);
            if (i+1 == PhotonNetwork.LocalPlayer.ActorNumber) 
            {
                Debug.LogError(PhotonNetwork.LocalPlayer.ActorNumber);
                portals[i].GetComponent<PhotonView>().RPC("SetRoomId", RpcTarget.AllBuffered, _userData.RoomID);
            }
        }
    }
}
