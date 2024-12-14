using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _region;
    [SerializeField] private InputField _roomName;

    [SerializeField] private ListItem _itemPrefab;
    [SerializeField] private Transform _content;

    [SerializeField] private string _nickName;

    private List<RoomInfo> _allRoomsInfo = new List<RoomInfo>();

    private GameObject _player;
    [SerializeField] private GameObject _playerPref;


    private void Start()
    {

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(_region);

        if (SceneManager.GetActiveScene().name == "game_scene")
        {
            _player = PhotonNetwork.Instantiate(_playerPref.name, Vector3.zero, Quaternion.identity);
        }

    }

    public override void OnConnectedToMaster()
    {

        Debug.Log("Вы подключились к: " + PhotonNetwork.CloudRegion);
        if (!string.IsNullOrEmpty(_nickName))
            PhotonNetwork.NickName = _nickName;
        else
            PhotonNetwork.NickName = "User";

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public void CreateRoomButton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(_roomName.text, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Создана комната, имя комнаты: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Не удалось создать комнату!");
    }



    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Вы отключены от сервера");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            foreach (var addInfo in _allRoomsInfo)
            {
                if (addInfo.masterClientId == info.masterClientId)
                    return;
            }
            ListItem listItem = Instantiate(_itemPrefab, _content);

            if (listItem != null)
            {
                listItem.SetInfo(info);
                _allRoomsInfo.Add(info);
            }

        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("game_scene");
    }

    public void JoinRandRoomButton()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinButton()
    {
        PhotonNetwork.JoinRoom(_roomName.text);
    }

    public void LeaveButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(_player.gameObject);
        PhotonNetwork.LoadLevel("main");
    }
}

