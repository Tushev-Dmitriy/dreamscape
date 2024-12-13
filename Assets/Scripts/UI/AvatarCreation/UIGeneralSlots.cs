using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGeneralSlots : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO setGenderEventChannel;
    [SerializeField] private List<UISelectButton> uiSelectButtons;

    private void OnEnable()
    {
        for (int i = 0; i < uiSelectButtons.Count; i++)
        {
            uiSelectButtons[i].Clicked += Select;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < uiSelectButtons.Count; i++)
        {
            uiSelectButtons[i].Clicked -= Select;
        }
    }
    
    private void Select(int id)
    {
        var uiSelectButton = uiSelectButtons[id];
        

        if (uiSelectButton != null)
        {
            uiSelectButton.Select();

            foreach (var button in uiSelectButtons)
            {
                if (button != uiSelectButton)
                {
                    button.Deselect();
                }
            }
            
            setGenderEventChannel.RaiseEvent(id);
        }
    }

}
