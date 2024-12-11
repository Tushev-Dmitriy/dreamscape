using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UIWorkList : MonoBehaviour
    {
        [SerializeField] private List<UIWorkItem> availableSlots = new List<UIWorkItem>();
        [SerializeField] private List<UIWorkSlot> workSlots = new List<UIWorkSlot>();
        
        [SerializeField] private VoidEventChannelSO getWorkListEventChannel;
        [SerializeField] private VoidEventChannelSO setWorkListEventChannel;
        [SerializeField] private VoidEventChannelSO setWorksInSlotsEvent;
        
        [SerializeField] private UserData userData;
        
        public UnityAction Closed;
        
        List<WorkItemStack> listItemsToShow = new List<WorkItemStack>();

        private void OnEnable()
        {
            getWorkListEventChannel.RaiseEvent();

            setWorkListEventChannel.OnEventRaised += SetItems;
        }

        private void OnDisable()
        {
            setWorkListEventChannel.OnEventRaised -= SetItems;
        }

        private void SetItems()
        {
            foreach (var work in userData.AllWorks)
            {
                var itemStack = new WorkItemStack(work);
                listItemsToShow.Add(itemStack);
            }
            
            FillItems(listItemsToShow);
        }

        private void FillItems(List<WorkItemStack> listItemsToShow)
        {
            if (availableSlots == null)
            {
                availableSlots = new List<UIWorkItem>();
            }
            
            int maxCount = Mathf.Max(listItemsToShow.Count, availableSlots.Count);
            
            for (int i = 0; i < maxCount; i++)
            {
                if (i < listItemsToShow.Count)
                {
                    availableSlots[i].SetItem(listItemsToShow[i]);

                }
                else if (i < availableSlots.Count)
                {
                    availableSlots[i].SetInactiveItem();
                }

            }
        }

        public void SaveChanges()
        {
            foreach (var workSlot in workSlots)
            {
                workSlot.SaveChanges();
            }
            
            setWorksInSlotsEvent.RaiseEvent();
        }
        
        public void CloseInventory()
        {
            Closed.Invoke();
        }
    }
}