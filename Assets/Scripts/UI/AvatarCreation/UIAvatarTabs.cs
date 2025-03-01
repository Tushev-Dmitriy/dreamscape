using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UI.TabBar;
using UnityEngine;
using UnityEngine.Events;

public class UIAvatarTabs : MonoBehaviour
{
    [SerializeField] private List<UIAvatarTab> tabBars = new List<UIAvatarTab>();
    public event UnityAction<AvatarTabSO> TabChanged;
    
    [SerializeField] private AvatarTabSO currentSelectedTab;
    
    private void OnEnable()
    {
        InitializeTabs(tabBars);
    }

    public void InitializeTabs(List<UIAvatarTab> tabList)
    {
        tabBars = tabList;

        foreach (var tab in tabBars)
        {
            if (tab != null)
            {
                tab.OnTabSelected += ChangeTab; 
            }
        }
    }

    public void SetTabs(List<AvatarTabSO> typesList, AvatarTabSO selectedTab)
    {
        for (int i = 0; i < tabBars.Count; i++)
        {
            if (i < typesList.Count)
            {
                tabBars[i].gameObject.SetActive(true);
            }
            else
            {
                tabBars[i].gameObject.SetActive(false);
            }
        }
    }

    public void SelectTab(AvatarTabSO selectedTab)
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

    public void DeselectTab(AvatarTabSO selectedTab)
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

    
    void ChangeTab(AvatarTabSO newTabType)
    {
        if (TabChanged != null)
        {
            TabChanged.Invoke(newTabType);
        }
        else
        {
            Debug.LogWarning("Нет подписчиков на событие TabChanged.");
        }
    }
}
