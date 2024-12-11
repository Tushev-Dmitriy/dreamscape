using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UIRating : MonoBehaviour
    {
        [SerializeField] private List<UIRatingItem> availableItemsSlots = new List<UIRatingItem>();
        [SerializeField] private Button closeButton;

        public UnityAction OnClose;

        public void FillItems()
        {
            
        }

        public void Close()
        {
            OnClose?.Invoke();
        }
    }
}