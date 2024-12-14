using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator _animator;
    private PhotonView _photonView;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _photonView = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _photonView.IsMine)
        {
            _animator.SetTrigger("ChangeColor");
        }
    }
}
