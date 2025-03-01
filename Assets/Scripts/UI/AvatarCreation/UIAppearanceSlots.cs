using System;
using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UnityEngine;

public class UIAppearanceSlots : MonoBehaviour
{
    [SerializeField] private AvatarDataSO avatarData;
    [SerializeField] private UserData _userData;

    [SerializeField] private List<UIItem> _itemSlots = default;

    [SerializeField] private IntEventChannelSO setHairstyleEventChannel;
    
    private List<AvatarItemStack> _slots = new List<AvatarItemStack>();

    private int selectedItemId = -1;

    private void OnEnable()
    {
        switch (_userData.AvatarData.Gender)
        {
            case 0:
                _slots = avatarData.MenItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.Hairstyle);
                break;
            case 1:
                _slots = avatarData.WomanItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.Hairstyle);
                break;
        }
        
        FillItems(_slots);
        
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].ItemSelected += InspectItem;
        }

    }
    
    private void OnDisable()
    {
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            _itemSlots[i].ItemSelected -= InspectItem;
        }
    }


    private void InspectItem(AvatarItemSO itemToInspect)
    {
        if (_itemSlots.Exists(o => o.currentItem.Item == itemToInspect))
        {
            int itemIndex = _itemSlots.FindIndex(o => o.currentItem.Item == itemToInspect);

            //unselect selected Item
            if (selectedItemId >= 0 && selectedItemId != itemIndex)
                UnselectItem(selectedItemId);

            //change Selected ID 
            selectedItemId = itemIndex;
            
            setHairstyleEventChannel.RaiseEvent(selectedItemId);
        }
    }
    
    void UnselectItem(int itemIndex)
    {
        if (_itemSlots.Count > itemIndex)
        {
            _itemSlots[itemIndex].UnselectItem();
        }
    }
    
    private void FillItems(List<AvatarItemStack> listItemsToShow)
    {
        if (_itemSlots == null)
        {
            _itemSlots = new List<UIItem>();
        }

        int itemsCount = listItemsToShow.Count;

        for (int i = 0; i < itemsCount; i++)
        {
            if (i < _itemSlots.Count)
            {
                bool isSelected = selectedItemId == i;
                _itemSlots[i].SetItem(listItemsToShow[i], isSelected);
            }
            else
            {
                Debug.LogError(
                    $"Not enough available slots for work items. Index {i} exceeds availableSlots count {_itemSlots.Count}");
            }
        }
    }
}