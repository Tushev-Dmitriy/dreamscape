using System;
using System.Collections;
using System.Collections.Generic;
using UI.TabBar;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITabBar : MonoBehaviour
{
    [SerializeField] private List<UITab> tabBars = new List<UITab>();
    public event UnityAction<TabSO> TabChanged;

    [SerializeField] private Text _playerName;
    [SerializeField] private UserData _userData;

    private bool tabsInitialized = false;

    private void OnEnable()
    {
        InitializeTabs(tabBars);

        _playerName.text = _userData.Login;
    }

    public void InitializeTabs(List<UITab> tabList)
    {
        tabBars = tabList;

        foreach (var tab in tabBars)
        {
            if (tab != null)
            {
                tab.OnTabSelected += ChangeTab; 
            }
        }

        tabsInitialized = true;
    }

    public void SetTabs(List<TabSO> typesList, TabSO selectedTab)
    {
        if (!tabsInitialized)
        {
            Debug.LogError("Tabs are not initialized. Call InitializeTabs first.");
            return;
        }
        
        for (int i = 0; i < tabBars.Count; i++)
        {
            if (i < typesList.Count)
            {
                //tabBars[i].SetTab(isSelected);
                tabBars[i].gameObject.SetActive(true);
            }
            else
            {
                tabBars[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectTab(TabSO selectedTab)
    {
        var tab = tabBars.Find(x => x.currentTabType == selectedTab);

        if (tab != null)
        {
            tab.SetTab(true);

            foreach (var otherTab in tabBars)
            {
                if (otherTab != tab)
                {
                    otherTab.SetTab(false);
                }
            }
        }
        else
        {
            Debug.LogError("Selected tab not found in tabBars.");
        }
    }

    public void DeselectTab(TabSO selectedTab)
    {
        var tab = tabBars.Find(x => x.currentTabType == selectedTab);

        if (tab != null)
        {
            tab.SetTab(false);
            
        }
        else
        {
            Debug.LogError("Selected tab not found in tabBars.");
        }
    }

    private void OnDisable()
    {
        foreach (var tab in tabBars)
        {
            if (tab != null)
            {
                tab.OnTabSelected -= ChangeTab;
            }
        }
    }

    
    void ChangeTab(TabSO newTabType)
    {
        TabChanged.Invoke(newTabType);
    }
}
