using System.Collections;
using Events;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public bool isRoom = false;
    public int roomId = 3;

    //public string photonRoomName;

    private RoomManager _photonRoomManager;

    [Header("Room")]
    [SerializeField] private LoadEventChannelSO loadRoomEventChannel;
    [SerializeField] private IntEventChannelSO currentRoomIdEvent;
    [SerializeField] private GameSceneSO gameScene;
    
    [Header("Hub")]
    [SerializeField] private LoadEventChannelSO loadHubEventChannel;
    [SerializeField] private GameSceneSO hubScene;
    
   /* [Header("Hub")]
    [SerializeField] private */

    public void Initialize(string photonRoomName, int roomId, RoomManager photonRoomManager)
    {
        //this.photonRoomName = photonRoomName;
        this.roomId = roomId;
        _photonRoomManager = photonRoomManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _photonRoomManager = FindObjectOfType<RoomManager>();
            _photonRoomManager.SetNextLoadRoom(roomId.ToString());
            _photonRoomManager.Disconect();


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

    [PunRPC]
    public void SetRoomId(int roomId)
    {
        this.roomId = roomId;
    }

}
