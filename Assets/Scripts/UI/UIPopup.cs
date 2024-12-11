using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public enum PopupType
    {
        Quit,
        Error
    }
    
    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private Button _buttonClose = default;
        [SerializeField] private UIGenericButton _confirmationButton = default;
        [SerializeField] private UIGenericButton _cancelButton = default;
        
        public PopupType _actualType;

        public event UnityAction<bool> ConfirmationResponseAction;
        public event UnityAction ClosePopupAction;
        
        private void OnDisable()
        {
            if (_actualType == PopupType.Quit)
            {
                _cancelButton.Clicked -= CancelButtonClicked;
                _confirmationButton.Clicked -= ConfirmButtonClicked;
            }
        }
        
        public void ClosePopupButtonClicked()
        {
            ClosePopupAction.Invoke();
        }

        void ConfirmButtonClicked()
        {
            ConfirmationResponseAction.Invoke(true);
        }

        void CancelButtonClicked()
        {
            ConfirmationResponseAction.Invoke(false);
        }
    }
}