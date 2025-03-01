using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class WorkSlot
{
    public string[] ImagesSlot = new string[3];
    public string[] MusicSlot = new string[3];
    public string[] ModelSlot = new string[3];
}

public class UIWorkSlot : MonoBehaviour
{
    [SerializeField] private InputField[] inputFields;
    [SerializeField] private WorkType workType;
    [SerializeField] private UserData userData;


    public UnityAction SaveChanges;

    private void OnEnable()
    {
        SaveChanges += SetWorksSlots;
    }

    private void OnDisable()
    {
        SaveChanges -= SetWorksSlots;
        ResetUI();
    }

    public void SetWorkSlot()
    {
        switch (workType)
        {
            case WorkType.Audio:

                if (userData.WorkSlot.MusicSlot != null)
                {
                    for (int i = 0; i < userData.WorkSlot.MusicSlot.Length; i++)
                    {
                        if (userData.WorkSlot.MusicSlot[i] != null)
                        {
                            inputFields[i].text = userData.WorkSlot.MusicSlot[i];
                        }
                        else
                        {
                            inputFields[i].text = "";
                        }
                    }
                }

                break;

            case WorkType.Image:
                if (userData.WorkSlot.ImagesSlot != null)
                {
                    for (int i = 0; i < userData.WorkSlot.ImagesSlot.Length; i++)
                    {
                        if (userData.WorkSlot.ImagesSlot[i] != null)
                        {
                            inputFields[i].text = userData.WorkSlot.ImagesSlot[i];
                        }
                        else
                        {
                            inputFields[i].text = "";
                        }
                    }
                }

                break;

            case WorkType.Model:

                if (userData.WorkSlot.ModelSlot != null)
                {
                    for (int i = 0; i < userData.WorkSlot.ModelSlot.Length; i++)
                    {
                        if (userData.WorkSlot.ModelSlot[i] != null)
                        {
                            inputFields[i].text = userData.WorkSlot.ModelSlot[i].ToString();
                        }
                        else
                        {
                            inputFields[i].text = "";
                        }
                    }
                }

                break;
        }
    }

    private void ResetUI()
    {
        foreach (var input in inputFields)
        {
            var textInput = input.GetComponentsInChildren<Text>()
                .First(c => c.gameObject.name.Contains("Text"));
            textInput.text = string.Empty;
        }
    }

    private void ColorField(InputField inputField, Color color)
    {
        var textInput = inputField.GetComponentsInChildren<Text>()
            .First(c => c.gameObject.name.Contains("Text"));
        textInput.color = color;
    }

    public void SetWorksSlots()
    {
        switch (workType)
        {
            case WorkType.Image:
                for (int i = 0; i < inputFields.Length; i++)
                {
                    ColorField(inputFields[i], Color.white);
                    if (int.TryParse(inputFields[i].text, out int id))
                    {
                        if (userData.AllWorks.Find(work => work.WorkID == id) != null)
                        {
                            userData.WorkSlot.ImagesSlot[i] = inputFields[i].text;
                        }
                    }
                    else
                    {
                        userData.WorkSlot.ImagesSlot[i] = null;
                    }
                }

                break;

            case WorkType.Audio:
                for (int i = 0; i < inputFields.Length; i++)
                {
                    ColorField(inputFields[i], Color.white);
                    if (int.TryParse(inputFields[i].text, out int id))
                    {
                        if (userData.AllWorks.Find(work => work.WorkID == id) != null)
                        {
                            userData.WorkSlot.MusicSlot[i] = inputFields[i].text;
                        }
                    }
                    else
                    {
                        userData.WorkSlot.MusicSlot[i] = null;
                    }
                }

                break;

            case WorkType.Model:
                for (int i = 0; i < inputFields.Length; i++)
                {
                    ColorField(inputFields[i], Color.white);
                    if (int.TryParse(inputFields[i].text, out int id))
                    {
                        if (userData.AllWorks.Find(work => work.WorkID == id) != null)
                        {
                            userData.WorkSlot.ModelSlot[i] = inputFields[i].text;
                        }
                    }
                    else
                    {
                        userData.WorkSlot.ModelSlot[i] = null;
                    }
                }

                break;
        }
    }
}