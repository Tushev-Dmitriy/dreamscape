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
            
            listItemsToShow.Clear();
        }

        private void SetItems()
        {
            listItemsToShow.Clear();
            
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
            
            foreach (var slot in availableSlots)
            {
                slot.SetInactiveItem();
            }

            int itemsCount = listItemsToShow.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                if (i < availableSlots.Count)
                {
                    availableSlots[i].SetItem(listItemsToShow[i]);
                }
                else
                {
                    Debug.LogError($"Not enough available slots for work items. Index {i} exceeds availableSlots count {availableSlots.Count}");
                }
            }

            // Deactivate remaining slots if there are more availableSlots than items
            for (int i = itemsCount; i < availableSlots.Count; i++)
            {
                availableSlots[i].SetInactiveItem();
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