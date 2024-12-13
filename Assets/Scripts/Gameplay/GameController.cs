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
    private bool isJoin = false;

    [SerializeField] private BoolEventChannelSO _onHubLoadedEvent;
    [SerializeField] private BoolEventChannelSO _onRoomLoadedEvent;
    
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private UserData UserData;

    [Header("test")]
    [SerializeField] private GameObject _playerPref;

    private void Awake()
    {
        _onHubLoadedEvent.OnEventRaised += OnHubSceneLoaded;
        _onRoomLoadedEvent.OnEventRaised += OnRoomSceneLoaded;
        currentRoomIdEvent.OnEventRaised += SetCurrentRoomId;
        RoomManager.onJoinedRoom += OnJoinedRoom;
    }

    private void OnDisable()
    {
        _onHubLoadedEvent.OnEventRaised -= OnHubSceneLoaded;
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
            
            Debug.Log("room controller");
        }
        else
        {
            Debug.LogError("RoomController not found in the scene!");
        }

        //SetPlayerPosition(new Vector3(0, 1, 0));
    }

    void OnHubSceneLoaded(bool onHubLoaded)
    {

        //SetPlayerPosition(new Vector3(0, 1, 0));
    }


    void SetPlayerPosition(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<MoveController>().SetPlayerPos();
        Camera.SetupCurrent(player.transform.GetChild(0).GetComponent<Camera>());
    }

    private void SetCurrentRoomId(int roomId)
    {
        UserData.CurrentRoomID = roomId;
    }

    private void OnJoinedRoom()
    {
        if (!isJoin)
        {
            Debug.LogError(0);
            isJoin = true;
            var player = PhotonNetwork.Instantiate(_playerPref.name, new Vector3(0, 1, 0), Quaternion.identity);
            var controller = player.GetComponent<MoveController>();
            controller.enabled = true;
            controller.currentCamera = player.transform.GetChild(0).GetComponent<Camera>();
            controller.currentCamera.gameObject.SetActive(true);
        }
    }
}