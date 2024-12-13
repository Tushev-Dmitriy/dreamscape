using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomOpener : MonoBehaviour, IInteractable
{
    [SerializeField] private RoomManager _roomManager;
    public string roomName;

    public string GetInteractText()
    {
        return "������������ � �������";
    }

    public void Interact()
    {
        _roomManager.SwitchToRoom(roomName);
    }

}
