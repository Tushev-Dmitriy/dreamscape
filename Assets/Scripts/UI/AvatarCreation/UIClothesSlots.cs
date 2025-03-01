using System;
using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UnityEngine;
using UnityEngine.Events;

public class UIClothesSlots : MonoBehaviour
{
    [SerializeField] private List<UIItem> _topItemSlots = default;
    [SerializeField] private List<UIItem> _downItemSlots = default;
    
    [SerializeField] private AvatarDataSO avatarData;
    [SerializeField] private UserData _userData;
    
    [Header("Events")]
    [SerializeField] private IntEventChannelSO setOutfitTopEventChannel;
    [SerializeField] private IntEventChannelSO setOutfitDownEventChannel;

    
    private List<AvatarItemStack> _topOutfits = new List<AvatarItemStack>();
    private List<AvatarItemStack> _downOutfits = new List<AvatarItemStack>();

    private int selectedItemId = -1;

    private void OnEnable()
    {
        switch (_userData.AvatarData.Gender)
        {
            case 0:
                _topOutfits = avatarData.MenItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitTop);
                _downOutfits = avatarData.MenItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitDown);
                break;
            case 1:
                _topOutfits = avatarData.WomanItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitTop);
                _downOutfits = avatarData.WomanItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitDown);
                break;
        }
        
        FillItems(_topOutfits, _downOutfits);
        
        ToggleItemEvents(_topItemSlots, InspectItem, true);
        ToggleItemEvents(_downItemSlots, InspectItem, true);

    }
    
    private void ToggleItemEvents(List<UIItem> itemSlots, UnityAction<AvatarItemSO> action, bool isSubscribe)
    {
        if (itemSlots == null) return;

        foreach (var slot in itemSlots)
        {
            if (slot == null) continue;

            if (isSubscribe)
                slot.ItemSelected += action;
            else
                slot.ItemSelected -= action;
        }
    }

    private void OnDisable()
    {
        ToggleItemEvents(_topItemSlots, InspectItem, false);
        ToggleItemEvents(_downItemSlots, InspectItem, false);
    }

    private void SetOutfit(List<UIItem> itemSlots, IntEventChannelSO eventChannel, AvatarItemSO itemToInspect)
    {
        int itemIndex = itemSlots.FindIndex(o => o.currentItem.Item == itemToInspect);
        if (itemIndex < 0) return;

        // Unselect previous item if needed
        if (selectedItemId >= 0 && selectedItemId != itemIndex)
            UnselectItem(selectedItemId);

        selectedItemId = itemIndex; // Update selected ID
        eventChannel.RaiseEvent(selectedItemId);
    }

    private void SetOutfitTop(AvatarItemSO itemToInspect) =>
        SetOutfit(_topItemSlots, setOutfitTopEventChannel, itemToInspect);

    private void SetOutfitDown(AvatarItemSO itemToInspect) =>
        SetOutfit(_downItemSlots, setOutfitDownEventChannel, itemToInspect);

    
    private void InspectItem(AvatarItemSO itemToInspect)
    {
        var avatarType = itemToInspect.AvatarItemType.AvatarType;

        switch (avatarType)
        {
            case AvatarType.OutfitTop:
                SetOutfitTop(itemToInspect);
                break;
            
            case AvatarType.OutfitDown:
                SetOutfitDown(itemToInspect);
                break;
        }
    }
    
    void UnselectItem(int itemIndex)
    {
        if (_topItemSlots.Count > itemIndex)
        {
            _topItemSlots[itemIndex].UnselectItem();
        }
        
        if (_downItemSlots.Count > itemIndex)
        {
            _downItemSlots[itemIndex].UnselectItem();
        }
    }
    
    private void FillItems(List<AvatarItemStack> outfitTop, List<AvatarItemStack> outfitDown)
    {
        for (int i = 0; i < _topItemSlots.Count; i++)
        {
            if (i < outfitTop.Count)
            {
                bool isSelected = selectedItemId == i;
                _topItemSlots[i].SetItem(outfitTop[i], isSelected);
            }
            else
            {
                _topItemSlots[i].SetInactiveItem();
            }
        }

        for (int i = 0; i < _downItemSlots.Count; i++)
        {
            if (i < outfitDown.Count)
            {
                bool isSelected = selectedItemId == i;
                _downItemSlots[i].SetItem(outfitDown[i], isSelected);
            }
            else
            {
                _downItemSlots[i].SetInactiveItem();
            }
        }
    }

}
