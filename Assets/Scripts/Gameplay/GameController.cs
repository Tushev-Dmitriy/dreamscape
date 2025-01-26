using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using Unity.VisualScripting;

public class GameController : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO _onRoomLoadedEvent;
    
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private UserData UserData;

    [Header("test")]
    [SerializeField] private GameObject _playerPref;
    [SerializeField] private GameObject _portalPref;
    [SerializeField] private RoomManager _photonRoomManager;

    private void Awake()
    {
        _onRoomLoadedEvent.OnEventRaised += OnRoomSceneLoaded;
        currentRoomIdEvent.OnEventRaised += SetCurrentRoomId;
        RoomManager.onJoinedRoom += OnJoinedRoom;
    }

    private void OnDisable()
    {
        _onRoomLoadedEvent.OnEventRaised -= OnRoomSceneLoaded;
        currentRoomIdEvent.OnEventRaised -= SetCurrentRoomId;
        RoomManager.onJoinedRoom -= OnJoinedRoom;
        
        UserData.ResetSlotsData();
    }

    private void OnRoomSceneLoaded(bool isLoaded)
    {
        var roomControllerSceneObject = FindObjectOfType<RoomController>();

        if (roomControllerSceneObject != null)
        {
            RoomWorksFetcher.Instance.roomController = roomControllerSceneObject;
            RoomWorksFetcher.Instance.StartGetRoom(UserData.CurrentRoomID);
        }
        else
        {
            Debug.LogError("RoomController not found in the scene!");
        }

    }

    private void SetCurrentRoomId(int roomId)
    {
        UserData.CurrentRoomID = roomId;
    }

    private void OnJoinedRoom()
    {
        SpawnPlayer();

    }

    private void SpawnPlayer()
    {
        var player = PhotonNetwork.Instantiate(_playerPref.name, new Vector3(0, 1, 0), Quaternion.identity);
        var controller = player.GetComponentInChildren<MoveController>();
        controller.enabled = true;
        controller.currentCamera.gameObject.SetActive(true);
        controller.currentCamera.enabled = true;
    }
}