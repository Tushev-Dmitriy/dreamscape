using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UI.TabBar;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAvatarTab : MonoBehaviour
{
    public UnityAction<AvatarTabSO> OnTabSelected;
    
    [SerializeField] private Button _actionButton = default;
    [SerializeField] private GameObject selectTabImage;
    [SerializeField] private GameObject unselectTabImage;

    [ReadOnly] public AvatarTabSO currentTabType = default;
    
    public void SetTab(bool isSelected)
    {
        UpdateState(isSelected);
    }

    public void UpdateState(bool isSelected)
    {
        _actionButton.interactable = !isSelected;
        
        if (isSelected)
        {
            selectTabImage.gameObject.SetActive(true);
        }
        else
        {
            selectTabImage.gameObject.SetActive(false);
        }
    }

    public void OnTabClicked()
    {
        OnTabSelected.Invoke(currentTabType);
    }
}
