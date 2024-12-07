using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

public class ConnectData : MonoBehaviour
{
    private static string host = "http://localhost:8000/";
    public string registrationUrl = $"{host}auth/register/";
    public string loginUrl = $"{host}auth/login/";
}
