using UI.TabBar;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITab : MonoBehaviour
{
    public UnityAction<TabSO> OnTabSelected;
    
    [SerializeField] private Button _actionButton = default;
    [SerializeField] private Image selectTabImage;

    [ReadOnly] public TabSO currentTabType = default;
    
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
