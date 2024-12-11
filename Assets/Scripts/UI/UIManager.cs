using System;
using Events;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UIHud uiHud;

        [SerializeField] private BoolEventChannelSO onHubLoadedEvent;
        [SerializeField] private LoadEventChannelSO _loadMenuEvent = default;

        private void OnEnable()
        {
            onHubLoadedEvent.OnEventRaised += SetUI;
        }

        private void OnDisable()
        {
            onHubLoadedEvent.OnEventRaised -= SetUI;
        }

        void SetUI(bool isLoaded)
        {
            uiHud.gameObject.SetActive(true);
            uiHud.SetTabBarTabs();
            Time.timeScale = 1;
        }
    }
}