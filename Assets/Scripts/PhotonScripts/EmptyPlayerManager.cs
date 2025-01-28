using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EmptyPlayerManager : MonoBehaviourPunCallbacks
{
    private const string EmptyPlayerNickname = "EmptyPlayer";

    private void Start()
    {
        // Если это хост комнаты
        if (PhotonNetwork.IsMasterClient)
        {
            AddEmptyPlayerIfNeeded();
        }
    }

    private void AddEmptyPlayerIfNeeded()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            // Устанавливаем пользовательское свойство комнаты для "пустого игрока"
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { EmptyPlayerNickname, true }
            });

            Debug.Log("Empty player added to the room.");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.PlayerList.Length == 1 && PhotonNetwork.IsMasterClient)
        {
            AddEmptyPlayerIfNeeded();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Удаляем пустого игрока из пользовательских свойств комнаты, если он есть
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(EmptyPlayerNickname))
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Remove(EmptyPlayerNickname);
            Debug.Log("Empty player removed from the room.");
        }
    }
}
