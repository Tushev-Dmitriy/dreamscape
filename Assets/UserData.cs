using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    [Header("User Information")]
    public int UserID;
    public string Login;
    public string Nickname;
}

[System.Serializable]
public class UserLoginResponse
{
    public int UserID;
    public string Login;
    public string Nickname;
}