using System;
using UnityEngine;

namespace UI.AvatarCreation
{
    [Serializable]
    public class AvatarItemStack
    {
        [SerializeField] private AvatarItemSO _item;

        public AvatarItemSO Item => _item;

        public AvatarItemStack()
        {
            _item = null;
        }
        
        public AvatarItemStack(AvatarItemStack itemStack)
        {
            _item = itemStack.Item;
        }
        
        public AvatarItemStack(AvatarItemSO item)
        {
            _item = item;
        }
    }
}