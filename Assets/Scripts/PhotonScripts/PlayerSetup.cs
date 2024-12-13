using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    private PlayerInteract _playerInteract;
    private Movement _movement;

    private string nickName;

    public GameObject camera;

    public TextMeshPro nickNameText;

    private void Awake()
    {
        _movement = GetComponent<Movement>();
        _playerInteract = GetComponent<PlayerInteract>();
    }

    public void IsLocalPlayer()
    {
        _movement.enabled = true;
        _playerInteract.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetNickName(string nickName)
    {
        this.nickName = nickName;

        nickNameText.text = nickName;
    }
}
