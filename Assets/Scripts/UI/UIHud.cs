using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UI;
using UI.TabBar;
using UnityEngine;

public class UIHud : MonoBehaviour
{
    [Header("UI Elements")] [SerializeField]
    private UITabBar tabBar;

    [SerializeField] private UIPopup exitConfirmationPopup;
    [SerializeField] private UIRating ratingPanel;
    [SerializeField] private UIWorkList workListPanel;
    [SerializeField] private UIAddWork addWorkPanel;

    [SerializeField] private GameSceneSO _hubScene;

    [SerializeField] private List<TabSO> _tabTypesList = new List<TabSO>();
    [SerializeField] private GameStateSO gameState;
    
    [SerializeField] private LoadEventChannelSO _backToHub;
    
    private TabSO _selectedTab = default;

    private GameObject _currentPanel = default;
    private GameObject _previousPanel = default;

    private void OnEnable()
    {
        tabBar.TabChanged += OnChangeTab;
        workListPanel.Closed += CloseWorkListPanel;
        addWorkPanel.Closed += CloseAddWorkPanel;
    }

    private void OnDisable()
    {
        tabBar.TabChanged -= OnChangeTab;
        workListPanel.Closed -= CloseWorkListPanel;
        addWorkPanel.Closed -= CloseAddWorkPanel;

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

    private void CloseWorkListPanel()
    {
        workListPanel.gameObject.SetActive(false);
        tabBar.DeselectTab(_selectedTab);

        _currentPanel = null;
        _selectedTab = null;
        
        gameState.UpdateGameState(GameState.Gameplay);
    }

    private void CloseAddWorkPanel()
    {
        addWorkPanel.gameObject.SetActive(false);
        tabBar.DeselectTab(_selectedTab);

        _currentPanel = null;
        _selectedTab = null;

        gameState.UpdateGameState(GameState.Gameplay);
    }

    void SetTabs(List<TabSO> typesList, TabSO selectedType)
    {
        tabBar.SetTabs(typesList, selectedType);
    }

    void SelectTab(TabSO tab)
    {
        tabBar.SelectTab(tab);
    }

    public void SetTabBarTabs()
    {
        SetTabs(_tabTypesList, _selectedTab);
    }

    private void ShowPanel(TabType _selectedTabType = default)
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
            case TabType.Rating:
                if (ratingPanel != null)
                    SetCurrentPanel(ratingPanel.gameObject);
                gameState.UpdateGameState(GameState.UI);
                break;
            
            case TabType.Exit:
                if (exitConfirmationPopup != null)
                    ShowExitConfirmationPopup();
                gameState.UpdateGameState(GameState.UI);
                break;
            
            case TabType.WorkList:
                if (workListPanel != null)
                    SetCurrentPanel(workListPanel.gameObject);
                gameState.UpdateGameState(GameState.UI);
                break;
            
            case TabType.Home:
                GoToHub();
                gameState.UpdateGameState(GameState.Gameplay);
                break;
            
            case TabType.AddWork:
                if (addWorkPanel != null)
                    SetCurrentPanel(addWorkPanel.gameObject);
                gameState.UpdateGameState(GameState.UI);
                break;
            
            default:
                Debug.LogError($"Unhandled TabType: {_selectedTabType}");
                break;
        }
    }
    
    private void OnChangeTab(TabSO type)
    {
        ShowPanel(type.TabType);
    }

    private void ShowExitConfirmationPopup()
    {
        exitConfirmationPopup.ConfirmationResponseAction += HideExitConfirmationPopup;
        exitConfirmationPopup.ClosePopupAction += HideExitConfirmationPopup;

        SetCurrentPanel(exitConfirmationPopup.gameObject);
        //exitConfirmationPopup.gameObject.SetActive(true);
    }

    private void GoToHub()
    {
        _backToHub.RaiseEvent(_hubScene, true, true);
    }

    private void HideExitConfirmationPopup(bool isConfirmed)
    {
        exitConfirmationPopup.ConfirmationResponseAction -= HideExitConfirmationPopup;
        exitConfirmationPopup.gameObject.SetActive(false);
        
        if (isConfirmed)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else
        {
            gameState.UpdateGameState(GameState.Gameplay);

        }
    }
}