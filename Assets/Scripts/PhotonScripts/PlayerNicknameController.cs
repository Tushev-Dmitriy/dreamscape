using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNicknameController : MonoBehaviour
{
    [SerializeField] private InputField _nickInputField;
    [SerializeField] private PlayerDataSO _playerData;    
    public void ChangeNick(string nick)
    {
        _playerData.playerName = nick;
    }
}
