using UnityEngine;

namespace UI.AvatarCreation
{
    public enum AvatarType
    {
        Hairstyle,
        OutfitTop,
        OutfitDown
    }
    
    [CreateAssetMenu(fileName = "Item Type", menuName = "Avatar/Item Type", order = 1)]

    public class AvatarItemTypeSO : ScriptableObject
    {
        [SerializeField] private AvatarType avatarType;
        [SerializeField] private AvatarTabSO avatarTab;
        
        public AvatarTabSO AvatarTab => avatarTab;
        public AvatarType AvatarType => avatarType;
    }
}