using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [CreateAssetMenu(fileName = "New Load Evet Channel", menuName = "Events/LoadEventChannelSO")]
    public class LoadEventChannelSO : ScriptableObject
    {
        public UnityAction<GameSceneSO, bool, bool> OnLoadingRequested;

        public void RaiseEvent(GameSceneSO locationToLoad, bool showLoadingScreen = false, bool fadeScreen = false)
        {
            if (OnLoadingRequested != null)
            {
                OnLoadingRequested.Invoke(locationToLoad, showLoadingScreen, fadeScreen);
            }
            else
            {
                Debug.LogWarning("A Scene loading was requested, but nobody picked it up. ");
            }
        }
    }
}