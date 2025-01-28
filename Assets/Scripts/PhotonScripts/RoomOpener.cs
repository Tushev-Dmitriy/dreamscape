using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomOpener : MonoBehaviour
{
    [SerializeField] private RoomManager _roomManager;
    public string roomName;

    public string GetInteractText()
    {
        return "Подключиться к комнате";
    }

    public void Interact()
    {
        _roomManager.SwitchToRoom(roomName);
    }
}
