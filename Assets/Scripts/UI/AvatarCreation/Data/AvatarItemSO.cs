using System.Collections;
using System.Collections.Generic;
using UI.AvatarCreation;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Avatar/Item", order = 1)]
public class AvatarItemSO : ScriptableObject
{
    [SerializeField] private Sprite preview;
    [SerializeField] private AvatarItemTypeSO avatarItemType;
    [SerializeField] private string objectName;

    public Sprite Preview => preview;
    public AvatarItemTypeSO AvatarItemType => avatarItemType;
    public string ObjectName => objectName;
}
