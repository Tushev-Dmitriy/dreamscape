using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClothesSlots : MonoBehaviour
{
    [SerializeField] private List<UIItem> _topItemSlots = default;
    [SerializeField] private List<UIItem> _downItemSlots = default;
    
    [Header("Events")]
    [SerializeField] private IntEventChannelSO setOutfitTopEventChannel;
    [SerializeField] private IntEventChannelSO setOutfitDownEventChannel;

    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
