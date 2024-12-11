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
}
