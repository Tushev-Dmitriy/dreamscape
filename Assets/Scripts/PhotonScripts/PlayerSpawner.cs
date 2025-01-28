using Photon.Pun.Demo.SlotRacer;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    public List<Transform> spawnPoints;
    
    private void Start()
    {
        RoomManager.onJoinedRoom += OnJoinedRoom;
    }

    private void OnDestroy()
    {
        RoomManager.onJoinedRoom -= OnJoinedRoom;
    }

    private void OnJoinedRoom()
    {
        Debug.Log("Player has joined the room.");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, GetSpawnPosition(), Quaternion.identity);
            EnablePlayerControl(player);
        }
        else
        {
            Debug.LogError("Player prefab is not assigned in RoomManager.");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count - 1)].position;
    }

    private void EnablePlayerControl(GameObject player)
    {
        var photonView = player.GetComponent<PhotonView>();

        if (photonView != null)
        {
         //   photonView.RPC("SetNickName", RpcTarget.AllBuffered, _playerData.playerName);
        }
        else
        {
            Debug.LogWarning("Player prefab does not have some script of control script.");
        }
    }
}
