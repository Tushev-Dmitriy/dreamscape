using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [SerializeField] private List<AvatarItemSO> defaultItems;
    
    private static Avatar _instance;

    public static Avatar Instance
    {
        get => _instance;

        set
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Avatar>();
            }
        }
    }

    public List<AvatarItemSO> DefaultItems => defaultItems;

}
