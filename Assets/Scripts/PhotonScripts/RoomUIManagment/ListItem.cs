
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    [SerializeField] private Text _textName;
    [SerializeField] private Text _textPlayerCount;

    public void SetInfo(RoomInfo info)
    {
        _textName.text = info.Name;
        _textPlayerCount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }

    public void JoinToListRoom()
    {
        PhotonNetwork.JoinRoom(_textName.text);
    }
}
