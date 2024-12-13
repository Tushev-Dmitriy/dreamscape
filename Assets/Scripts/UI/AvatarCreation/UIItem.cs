using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    [SerializeField] private Image _itemPreviewImage;
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _imgSelected;
    [SerializeField] private Button _itemButton;

    [SerializeField] private string _objectName;

    public UnityAction<AvatarItemSO> ItemSelected;
    
    [HideInInspector] public AvatarItemStack currentItem;
    
    private bool _isSelected = false;

    public void SetItem(AvatarItemStack itemStack, bool isSelected)
    {
        _isSelected = isSelected;
        currentItem = itemStack;

        
        _imgSelected.gameObject.SetActive(isSelected);
    }

    public void SetInactiveItem()
    {
        _itemPreviewImage.gameObject.SetActive(false);
        _bgImage.gameObject.SetActive(false);
        _imgSelected.gameObject.SetActive(false);
    }
    
    public void SelectFirstElement()
    {
        _isSelected = true;
        _itemButton.Select();
        SelectItem();
    }

    public void SelectItem()
    {
        _isSelected = true;

        if (ItemSelected != null)
        {
            _imgSelected.gameObject.SetActive(true);
            ItemSelected.Invoke(currentItem.Item);
        }
        else
        {
            _imgSelected.gameObject.SetActive(false);
        }
    }

    public void UnselectItem()
    {
        _isSelected = false;
        _imgSelected.gameObject.SetActive(false);
    }
}
