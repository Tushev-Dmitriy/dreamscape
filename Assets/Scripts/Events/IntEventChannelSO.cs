using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New event", menuName = "Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public UnityAction<int> OnEventRaised;

    public void RaiseEvent(int number)
    {
        if (OnEventRaised != null)
        {
            OnEventRaised.Invoke(number);
        }
        else
        {
            Debug.LogError($"There is no subscribers to this event: {name}");
        }
    }
}
