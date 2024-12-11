using System;
using UnityEngine;

namespace UI
{
    [Serializable]
    public class WorkItemStack
    {
        [SerializeField] private AllWork _item;

        public AllWork Item => _item;

        public WorkItemStack()
        {
            _item = null;
        }
        
        public WorkItemStack(WorkItemStack itemStack)
        {
            _item = itemStack.Item;
        }
        
        public WorkItemStack(AllWork item)
        {
            _item = item;
        }
    }
}