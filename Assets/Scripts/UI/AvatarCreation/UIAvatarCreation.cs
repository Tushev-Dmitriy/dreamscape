using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UI.AvatarCreation;
using UnityEngine;

public class UIAvatarCreation : MonoBehaviour
{
    [Header("UI Elements")] 
    [SerializeField] private UIAvatarTabs tabBar;
    
    [SerializeField] private UIClothesSlots _clothesSlots;
    [SerializeField] private UIAppearanceSlots _appearanceSlots;
    [SerializeField] private UIGeneralSlots _generalSlots;
    
    [SerializeField] private List<AvatarTabSO> _tabTypesList = new List<AvatarTabSO>();
    [SerializeField] private GameStateSO gameState;
    
    [SerializeField] private LoadEventChannelSO _backToMenu;
    [SerializeField] private VoidEventChannelSO onSaveAvatarDataEvent;

    [SerializeField] private GameSceneSO _menuToLoad;
    
    private AvatarTabSO _selectedTab = default;

    private GameObject _currentPanel = default;
    private GameObject _previousPanel = default;
    

    private void Awake()
    {
        if (tabBar == null)
        {
            Debug.LogError("TabBar не установлен в инспекторе.");
            return;
        }

        tabBar.TabChanged += OnChangeTab; // Подписка на событие
        Debug.Log("Подписка на TabChanged выполнена в Start.");
    
        SetTabs(_tabTypesList, _selectedTab);
    }

    private void OnDisable()
    {
        tabBar.TabChanged -= OnChangeTab;
    }
    
    private void OnChangeTab(AvatarTabSO type)
    {
        ShowPanel(type.TabType);
    }
    
    private void ShowPanel(AvatarTabType _selectedTabType = default)
    {
        if (!_tabTypesList.Exists(o => o.TabType == _selectedTabType))
        {
            Debug.LogError($"TabType {_selectedTabType} not found in _tabTypesList.");
            return;
        }

        _selectedTab = _tabTypesList.Find(o => o.TabType == _selectedTabType);
        if (_selectedTab == null)
        {
            Debug.LogError("No TabSO selected.");
            return;
        }

        SelectTab(_selectedTab);

        switch (_selectedTabType)
        {
            case AvatarTabType.Clothes:
                if (_clothesSlots != null)
                    SetCurrentPanel(_clothesSlots.gameObject);
                break;
            
            case AvatarTabType.General:
                if (_generalSlots != null)
                    SetCurrentPanel(_generalSlots.gameObject);
                break;
            
            case AvatarTabType.Appearance:
                if (_appearanceSlots != null)
                    SetCurrentPanel(_appearanceSlots.gameObject);
                break;
            
            default:
                Debug.LogError($"Unhandled TabType: {_selectedTabType}");
                break;
        }
    }
    
    void SetTabs(List<AvatarTabSO> typesList, AvatarTabSO selectedType)
    {
        tabBar.SetTabs(typesList, selectedType);
    }
    
    void SelectTab(AvatarTabSO tab)
    {
        tabBar.SelectTab(tab);
    }
    
    public void BackToMenu()
    {
        _backToMenu.RaiseEvent(_menuToLoad, true, true);
    }

    public void SaveAvatarData()
    {
        onSaveAvatarDataEvent.RaiseEvent();
    }

    private void SetCurrentPanel(GameObject panel)
    {
        if (_currentPanel != null)
        {
            _previousPanel = _currentPanel;
            _previousPanel.SetActive(false);
        }

        _currentPanel = panel;
        _currentPanel.SetActive(true);
    }
    

}
