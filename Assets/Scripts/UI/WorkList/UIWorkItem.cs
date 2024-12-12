using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public enum WorkType
{
    Image,
    Audio,
    Model
}

public class UIWorkItem : MonoBehaviour
{
    [SerializeField] private Text idWork;
    [SerializeField] private Text titleWork;
    
    [SerializeField] private Image iconPreview;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image moderationIcon;
    
    [SerializeField] private Sprite[] icons = new Sprite[3];
    
    [HideInInspector] public WorkItemStack currentItem;
    
    [SerializeField] private IntEventChannelSO deleteItemEventChannel;
    
    [SerializeField] private VoidEventChannelSO getWorkListEventChannel;
    [SerializeField] private VoidEventChannelSO setWorkListEventChannel;
    
    public void DeleteItem()
    {
        deleteItemEventChannel.RaiseEvent(currentItem.Item.WorkID);
        
        getWorkListEventChannel.RaiseEvent();
    }

    public void SetItem(WorkItemStack itemStack)
    {
        currentItem = itemStack;
        
        idWork.gameObject.SetActive(true);
        titleWork.gameObject.SetActive(true);
        bgImage.gameObject.SetActive(true);
        iconPreview.gameObject.SetActive(true);
        moderationIcon.gameObject.SetActive(true);

        idWork.text = itemStack.Item.WorkID.ToString();
        titleWork.text = itemStack.Item.WorkTitle;

        switch (itemStack.Item.WorkType.ToLower())
        {
            case "image":
                iconPreview.sprite = icons[0];
                break;
            case "music":
                iconPreview.sprite = icons[1];
                break;
            case "model":
                iconPreview.sprite = icons[2];
                break;
        }

        if (itemStack.Item.IsModerated)
        {
            moderationIcon.gameObject.SetActive(true);
        }
        else
        {
            moderationIcon.gameObject.SetActive(false);
        }
    }
    
    public void SetInactiveItem()
    {
        currentItem = null;
        idWork.gameObject.SetActive(false);
        titleWork.gameObject.SetActive(false);
        bgImage.gameObject.SetActive(false);
        iconPreview.gameObject.SetActive(false);
        moderationIcon.gameObject.SetActive(false);
    }
}
