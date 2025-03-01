using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class UIGenericButton : MonoBehaviour
    {
        public UnityAction Clicked = default;
        
        public void Click()
        {
            Clicked.Invoke();
        }
    }
}