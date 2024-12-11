using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BoolEventChannelSO _onHubLoadedEvent;
    [SerializeField] private BoolEventChannelSO _onRoomLoadedEvent;
    
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private UserData UserData;

    private void Awake()
    {
        _onHubLoadedEvent.OnEventRaised += OnHubSceneLoaded;
        _onRoomLoadedEvent.OnEventRaised += OnRoomSceneLoaded;
        currentRoomIdEvent.OnEventRaised += SetCurrentRoomId;
    }

    private void OnDisable()
    {
        _onHubLoadedEvent.OnEventRaised -= OnHubSceneLoaded;
        _onRoomLoadedEvent.OnEventRaised -= OnRoomSceneLoaded;
        currentRoomIdEvent.OnEventRaised -= SetCurrentRoomId;
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

        SetPlayerPosition(new Vector3(0, 1, 0));
    }

    void OnHubSceneLoaded(bool onHubLoaded)
    {
        SetPlayerPosition(new Vector3(0, 1, 0));
    }

    void SetPlayerPosition(Vector3 position)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<MoveController>().SetPlayerPos();
    }

    private void SetCurrentRoomId(int roomId)
    {
        UserData.CurrentRoomID = roomId;
    }
}