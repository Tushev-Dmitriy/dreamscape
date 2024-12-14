using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text _textLastMessage;
    [SerializeField] private InputField _textMessageField;

    private PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
    }

    public void SendButton()
    {
        _photonView.RPC("Send_Data", RpcTarget.AllBuffered, PhotonNetwork.NickName, _textMessageField.text);
    }

    [PunRPC]
    private void Send_Data(string nick, string message)
    {
        _textLastMessage.text = nick + ": " + message;
    }
}
