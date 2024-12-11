using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    [CreateAssetMenu(fileName = "WorkUploadHandlerSO", menuName = "Events/WorkUploadHandlerSO", order = 0)]
    public class WorkUploadHandlerSO : ScriptableObject
    {
        public UnityAction<UI.Work> OnEventRaised;

        public void RaiseEvent(UI.Work work)
        {
            if (OnEventRaised != null)
            {
                OnEventRaised.Invoke(work);
            }
        }
    }
}