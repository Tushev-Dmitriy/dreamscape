using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomManager : MonoBehaviour
{
    public Camera waitingCamera;

    private void Start()
    {
        RoomManager.onJoinedRoom += OnJoinedRoom;
        RoomManager.onLeftRoom += OnLeftRoom;
    }

    private void OnDestroy()
    {
        RoomManager.onJoinedRoom -= OnJoinedRoom;
        RoomManager.onLeftRoom -= OnLeftRoom;
    }

    private void OnJoinedRoom()
    {
        Debug.Log("Joined room, disabling waiting camera...");
        DisableWaitingCamera();
    }

    private void OnLeftRoom()
    {
        Debug.Log("Left room, enabling waiting camera...");
        if (waitingCamera != null)
        {
            waitingCamera.gameObject.SetActive(true);
        }
    }

    private void DisableWaitingCamera()
    {
        if (waitingCamera != null)
        {
            waitingCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Waiting camera is not assigned in ConnectionCameraHandler.");
        }
    }


}
