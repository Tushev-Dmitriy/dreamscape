using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assimp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class RoomController : MonoBehaviour
{
    [HideInInspector] public RoomWorksResponse roomWorksResponse;
    public List<GameObject> workSlots;
    [SerializeField] FBXToGLTFConverter modelConverter;

    private List<string> savedFiles = new List<string>();

    public void SetWorksInRoom(RoomWorksResponse roomWorksResponseIn)
    {

        Debug.LogError("set works in room");
        roomWorksResponse = roomWorksResponseIn;

        List<Work> allWorkInRoom = roomWorksResponse.Works;

        for (int i = 0; i < workSlots.Count; i++)
        {
            if (i >= allWorkInRoom.Count || allWorkInRoom[i].WorkID == -1)
            {
                ClearSlot(workSlots[i]);
                continue;
            }

            Work tempWork = allWorkInRoom[i];

            string filePath = SaveWorkToFile(tempWork);
            if (string.IsNullOrEmpty(filePath))
                continue;

            switch (tempWork.WorkType.ToLower())
            {
                case "image":
                    SetImageInSlot(workSlots[i], filePath);
                    break;

                case "music":
                    SetAudioInSlot(workSlots[i], filePath);
                    break;

                case "model":
                    LoadModelWithMaterials(workSlots[i], filePath);
                    break;

                default:
                    Debug.LogWarning($"Неизвестный тип работы: {tempWork.WorkType}");
                    break;
            }
        }
    }

    private string SaveWorkToFile(Work work)
    {
        try
        {
            string extension = work.WorkType.ToLower() switch
            {
                "image" => "png",
                "music" => "mp3",
                "model" => "fbx",
                _ => "dat"
            };

            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{work.WorkTitle}_{timestamp}.{extension}";

            string savePath = Path.Combine(Application.persistentDataPath, fileName);

            byte[] fileData = System.Convert.FromBase64String(work.WorkContent);

            File.WriteAllBytes(savePath, fileData);

            savedFiles.Add(savePath);

            Debug.Log($"Файл {fileName} успешно сохранен в {savePath}");

            return savePath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка сохранения файла для работы {work.WorkID}: {ex.Message}");
            return null;
        }
    }

    private void SetImageInSlot(GameObject slot, string filePath)
    {
        byte[] imageData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageData))
        {
            UnityEngine.Material material = new UnityEngine.Material(Shader.Find("Standard"));
            material.mainTexture = texture;

            MeshRenderer renderer = slot.GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
        }
        else
        {
            Debug.LogError($"Не удалось загрузить текстуру из файла: {filePath}");
        }
    }

    private void SetAudioInSlot(GameObject slot, string filePath)
    {
        AudioSource audioSource = slot.GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            audioSource = slot.AddComponent<AudioSource>();
        }

        StartCoroutine(LoadAudioClip(filePath, audioSource));
    }

    private IEnumerator LoadAudioClip(string filePath, AudioSource audioSource)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                Debug.Log($"Аудио успешно загружено из {filePath}");
            }
            else
            {
                Debug.LogError($"Ошибка загрузки аудио из {filePath}: {www.error}");
            }
        }
    }

    private void ClearSlot(GameObject slot)
    {
        MeshRenderer renderer = slot.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material = null;
        }

        AudioSource audioSource = slot.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }

    private void LoadModelWithMaterials(GameObject slot, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Файл не найден: {filePath}");
            return;
        }

        modelConverter.ConvertAndAddModel(slot, filePath);
    }

    private void OnDestroy()
    {
        string directoryPath = Application.persistentDataPath;

        string[] files = Directory.GetFiles(directoryPath);
        foreach (string file in files)
        {
            File.Delete(file);
        }

        string[] directories = Directory.GetDirectories(directoryPath);
        foreach (string directory in directories)
        {
            Directory.Delete(directory, true);
        }
    }
}
