using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.AvatarCreation
{
    public class CharacterController : MonoBehaviour
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

        private void OnEnable()
        {
            SetGender(userData.AvatarData.Gender);
            SetHairstyle(userData.AvatarData.HairStyle);
            SetOutfitTop(userData.AvatarData.OutfitTop);
            SetOutfitDown(userData.AvatarData.OutfitDown);
        }

        private void SetGender(int gender)
        {
            _gender = (Gender)gender;
            
            switch (_gender)
            {
                case Gender.Man:
                    womanObject.SetActive(false);
                    menObject.SetActive(true);

                    currentGameobject = menObject;

                    currentItems = avatarData.MenItems;
                    break;
                
                case Gender.Woman:
                    menObject.SetActive(false);
                    womanObject.SetActive(true);

                    currentGameobject = womanObject;
                    
                    currentItems = avatarData.WomanItems;
                    break;
            }

            userData.AvatarData.Gender = gender;
        }

        private void SetHairstyle(int hairstyle)
        {
            hairstyleItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.Hairstyle);
            
            List<GameObject> hairstyleItemList = new List<GameObject>();

            foreach (var hair in hairstyleItems)
            {
                var character = currentGameobject.transform.GetChild(0);
                var hairObject = character.GetComponentsInChildren<Transform>(true) // true - учитывает все дочерние объекты
                    .FirstOrDefault(t => t.name == hair.Item.ObjectName)?.gameObject;      
                
                hairstyleItemList.Add(hairObject);
            }

            for (int i = 0; i < hairstyleItemList.Count; i++)
            {
                if (i == hairstyle)
                {
                    hairstyleItemList[i].SetActive(true);
                }
                else
                {
                    hairstyleItemList[i].SetActive(false);
                }
            }
        }

        private void SetOutfitTop(int outfitTop)
        {
            outfitTopItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitTop);
            
            List<GameObject> outfitTopObjects = new List<GameObject>();

            foreach (var outfit in outfitTopItems)
            {
                var character = currentGameobject.transform.GetChild(0);
                var outfitObject = character.GetComponentsInChildren<Transform>(true) // true - учитывает все дочерние объекты
                    .FirstOrDefault(t => t.name == outfit.Item.ObjectName)?.gameObject;      
                
                outfitTopObjects.Add(outfitObject);
            }

            for (int i = 0; i < outfitTopObjects.Count; i++)
            {
                if (i == outfitTop)
                {
                    outfitTopObjects[i].SetActive(true);
                }
                else
                {
                    outfitTopObjects[i].SetActive(false);
                }
            }
        }

        private void SetOutfitDown(int outfitDown)
        {
            outfitDownItems = currentItems.FindAll(x => x.Item.AvatarItemType.AvatarType == AvatarType.OutfitDown);
            
            List<GameObject> outfitDownObjects = new List<GameObject>();

            foreach (var outfit in outfitDownItems)
            {
                var character = currentGameobject.transform.GetChild(0);
                var outfitObject = character.GetComponentsInChildren<Transform>(true) // true - учитывает все дочерние объекты
                    .FirstOrDefault(t => t.name == outfit.Item.ObjectName)?.gameObject;      
                
                outfitDownObjects.Add(outfitObject);
            }

            for (int i = 0; i < outfitDownObjects.Count; i++)
            {
                if (i == outfitDown)
                {
                    outfitDownObjects[i].SetActive(true);
                }
                else
                {
                    outfitDownObjects[i].SetActive(false);
                }
            }
        }
    }
}