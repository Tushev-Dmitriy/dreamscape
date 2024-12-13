using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Avatar/Data", order = 1)]
public class AvatarDataSO : ScriptableObject
{
    [SerializeField] private List<AvatarItemStack> menItems;
    [SerializeField] private List<AvatarItemStack> womanItems;
    
    public List<AvatarItemStack> MenItems => menItems;
    public List<AvatarItemStack> WomanItems => womanItems;
}
