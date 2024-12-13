using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObserver : MonoBehaviour
{
    private void Start()
    {
        RoomEventHandler.onConnectedToMaster += OnConnectedToMaster;
    }

    private void OnConnectedToMaster()
    {
        transform.localScale = Vector3.one * 3;
    }

}
