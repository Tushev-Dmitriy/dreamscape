using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubOpener : MonoBehaviour
{
    [SerializeField] private RoomManager _roomManager;

    public string GetInteractText()
    {
        return "������������ � �������";
    }

    public void Interact()
    {
        _roomManager.SwitchToHub();
    }
}
