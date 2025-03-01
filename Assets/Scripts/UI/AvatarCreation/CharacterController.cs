using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace UI.AvatarCreation
{
    public class CharacterController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private UserData userData;
        [SerializeField] private AvatarDataSO avatarData;

        [SerializeField] private GameObject menObject;
        [SerializeField] private GameObject womanObject;
        
        private Gender _gender = Gender.Man;
        
        private List<AvatarItemStack> currentItems = new List<AvatarItemStack>();
        private GameObject currentGameobject;
        
        private List<AvatarItemStack> hairstyleItems = new List<AvatarItemStack>();
        private List<AvatarItemStack> outfitTopItems = new List<AvatarItemStack>();
        private List<AvatarItemStack> outfitDownItems = new List<AvatarItemStack>();

         private void Awake()
        {
            if (photonView.IsMine)
            {
                // Сохранение данных в CustomProperties при создании аватара
                Hashtable playerProperties = new Hashtable
                {
                    { "Gender", (int)userData.AvatarData.Gender },
                    { "HairStyle", userData.AvatarData.HairStyle },
                    { "OutfitTop", userData.AvatarData.OutfitTop },
                    { "OutfitDown", userData.AvatarData.OutfitDown }
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
            }
            else
            {
                // Получение данных другого игрока
                ApplyAvatarSettings(photonView.Owner);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer == photonView.Owner)
            {
                ApplyAvatarSettings(targetPlayer);
            }
        }

        private void ApplyAvatarSettings(Player player)
        {
            if (player.CustomProperties.TryGetValue("Gender", out object gender))
                SetGender((int)gender);

            if (player.CustomProperties.TryGetValue("HairStyle", out object hairStyle))
                SetHairstyle((int)hairStyle);

            if (player.CustomProperties.TryGetValue("OutfitTop", out object outfitTop))
                SetOutfitTop((int)outfitTop);

            if (player.CustomProperties.TryGetValue("OutfitDown", out object outfitDown))
                SetOutfitDown((int)outfitDown);
        }

        private void SetGender(int gender)
        {
            _gender = (Gender)gender;

            currentGameobject = _gender == Gender.Man ? menObject : womanObject;
            currentItems = _gender == Gender.Man ? avatarData.MenItems : avatarData.WomanItems;

            currentGameobject.SetActive(true);
        }

        private void SetHairstyle(int hairstyle)
        {
            hairstyleItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.Hairstyle);
            ActivateItem(hairstyleItems, hairstyle);
        }

        private void SetOutfitTop(int outfitTop)
        {
            outfitTopItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitTop);
            ActivateItem(outfitTopItems, outfitTop);
        }

        private void SetOutfitDown(int outfitDown)
        {
            outfitDownItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitDown);
            ActivateItem(outfitDownItems, outfitDown);
        }

        private void ActivateItem(List<AvatarItemStack> itemList, int index)
        {
            List<GameObject> itemObjects = new List<GameObject>();

            foreach (var item in itemList)
            {
                var character = currentGameobject.transform.GetChild(0);
                var itemObject = character.GetComponentsInChildren<Transform>(true)
                    .FirstOrDefault(t => t.name == item.Item.ObjectName)?.gameObject;

                if (itemObject != null)
                    itemObjects.Add(itemObject);
            }

            for (int i = 0; i < itemObjects.Count; i++)
            {
                itemObjects[i]?.SetActive(i == index);
            }
        }
    }
}