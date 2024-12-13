using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "UserData", order = 1)]
public class UserData : ScriptableObject
{
    [Header("User Information")]
    public int UserID;
    public string Login;
    public string Nickname;
    
    public int RoomID;
    public int CurrentRoomID;
    public List<int> WorksID = new List<int>();
    public List<int> LikedWorks = new List<int>();
    
    public List<AllWork> AllWorks = new List<AllWork>();

    public WorkSlot WorkSlot;
    public AvatarData AvatarData;
    
    public void ResetAvatarData()
    {
        AvatarData = null;
    }

    public void ResetSlotsData()
    {
        Array.Clear(WorkSlot.ImagesSlot, 0, WorkSlot.ImagesSlot.Length);
        Array.Clear(WorkSlot.MusicSlot, 0, WorkSlot.MusicSlot.Length);
        Array.Clear(WorkSlot.ModelSlot, 0, WorkSlot.ModelSlot.Length);
    }
}
