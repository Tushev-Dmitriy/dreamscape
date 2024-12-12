using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public bool isRoom = false;
    public int roomId = 3;

    [Header("Room")]
    [SerializeField] private LoadEventChannelSO loadRoomEventChannel;
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private GameSceneSO gameScene;
    
    [Header("Hub")]
    [SerializeField] private LoadEventChannelSO loadHubEventChannel;
    [SerializeField] private GameSceneSO hubScene;
    
   /* [Header("Hub")]
    [SerializeField] private */

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!isRoom) {
                currentRoomIdEvent.RaiseEvent(roomId);
                loadRoomEventChannel.RaiseEvent(gameScene, true, true);
            }
            else
            {
                loadHubEventChannel.RaiseEvent(hubScene, true, true);
            }
        }
    }
}
