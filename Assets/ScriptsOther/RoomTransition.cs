using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public bool isRoom = false;
    public int roomId = 3;

    [SerializeField] private LoadEventChannelSO loadRoomEventChannel;
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private GameSceneSO gameScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            currentRoomIdEvent.RaiseEvent(roomId);
            loadRoomEventChannel.RaiseEvent(gameScene, true, true);
        }
    }
}
