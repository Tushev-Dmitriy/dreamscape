using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    [Header("User Information")]
    public int UserID;
    public string Login;
    public string Nickname;
    public int RoomID;
    public int CurrentRoomID;
    public List<int> WorksID = new List<int>();
    public List<int> LikedWorks = new List<int>();
    [SerializeField] private RoomWorksFetcher roomWorksFetcher;

    [Header("Events")]
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;

    private void Start()
    {
        currentRoomIdEvent.OnEventRaised += SetCurrentRoomID;
    }

    private void OnDisable()
    {
        currentRoomIdEvent.OnEventRaised -= SetCurrentRoomID;
    }

    private void SetCurrentRoomID(int roomID)
    {
        CurrentRoomID = roomID;
        roomWorksFetcher.StartGetRoom(CurrentRoomID);
    }
}

[System.Serializable]
public class UserLoginResponse
{
    public int UserID;
    public string Login;
    public string Nickname;
    public int RoomID;
}