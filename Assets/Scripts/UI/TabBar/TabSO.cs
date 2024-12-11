using UnityEngine;

namespace UI.TabBar
{
    public enum TabType
    {
        Rating,
        AddWork,
        WorkList,
        Home,
        Exit
    }
    
    [CreateAssetMenu(fileName = "Tab", menuName = "TabBar/TabBar", order = 1)]
    public class TabSO : ScriptableObject
    {
        [SerializeField] private TabType tabType;

        public TabType TabType => tabType;
    }
}