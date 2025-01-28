using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubOpener : MonoBehaviour
{
    [SerializeField] private RoomManager _roomManager;

    public string GetInteractText()
    {
        return "Подключиться к комнате";
    }

    public void Interact()
    {
        _roomManager.SwitchToHub();
    }
}
