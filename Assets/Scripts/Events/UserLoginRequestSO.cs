using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [CreateAssetMenu(fileName = "User Login Request",menuName = "Events/UserLoginRequestSO")]
    public class UserLoginRequestSO : ScriptableObject
    {
        public UnityAction<string, string> OnEventRaised;

        public void RaiseEvent(string username, string password)
        {
            OnEventRaised?.Invoke(username, password);
        }
    }
}