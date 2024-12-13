using UI.TabBar;
using UnityEngine;

namespace UI.AvatarCreation
{
    public enum AvatarTabType
    {
        General,
        Clothes,
        Appearance
    }
    
    [CreateAssetMenu(fileName = "Tab", menuName = "Avatar/Tab", order = 1)]
    public class AvatarTabSO : ScriptableObject
    {
        [SerializeField] private AvatarTabType tabType;

        public AvatarTabType TabType => tabType;
    }
}