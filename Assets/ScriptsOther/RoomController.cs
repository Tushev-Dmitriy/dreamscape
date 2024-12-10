using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using TriLibCore.General;
using UnityEngine;
using UnityEngine.Networking;

public class RoomController : MonoBehaviour
{
    [HideInInspector] public RoomWorksResponse roomWorksResponse;
    public List<GameObject> workSlots;

    private List<string> savedFiles = new List<string>();

    public void SetWorksInRoom(RoomWorksResponse roomWorksResponseIn)
    {
        roomWorksResponse = roomWorksResponseIn;

        List<Work> allWorkInRoom = roomWorksResponse.Works;

        for (int i = 0; i < workSlots.Count; i++)
        {
            // Проверяем, есть ли работа для текущего слота
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
                    LoadModelWithTriLib(filePath, workSlots[i]);
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

            string savePath = Path.Combine(Application.streamingAssetsPath, fileName);

            byte[] fileData = System.Convert.FromBase64String(work.WorkContent);

            File.WriteAllBytes(savePath, fileData);

            savedFiles.Add(savePath); // Добавляем файл в список для последующего удаления

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
            Material material = new Material(Shader.Find("Standard"));
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
        // Удаляем материал, если есть
        MeshRenderer renderer = slot.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material = null;
        }

        // Удаляем аудиоклип, если есть
        AudioSource audioSource = slot.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }

    private void LoadModelWithTriLib(string filePath, GameObject slot)
    {
        var options = AssetLoader.CreateDefaultLoaderOptions(); // Настройки загрузчика

        AssetLoader.LoadModelFromFile(
            filePath,
            onLoad: (context) =>
            {
                GameObject loadedGameObject = context.RootGameObject; // Получаем загруженную модель
                if (loadedGameObject != null)
                {
                    loadedGameObject.transform.SetParent(slot.transform);
                    loadedGameObject.transform.localPosition = new Vector3(0, -0.5f, 0); // Позиция по y -0.5
                    loadedGameObject.transform.localRotation = Quaternion.Euler(0, -90, 0); // Поворот по y на -90 градусов
                    loadedGameObject.transform.localScale = new Vector3(3, 3, 3); // Масштаб 3x3x3

                    Debug.Log($"Модель успешно загружена в слот: {slot.name}");
                }
            },
            onMaterialsLoad: null, // Вызывается после загрузки материалов
            onProgress: (context, progress) =>
            {
                Debug.Log($"Прогресс загрузки модели: {progress * 100:F2}%");
            },
            onError: (error) =>
            {
                Debug.LogError($"Ошибка загрузки модели: {error}");
            },
            wrapperGameObject: null, // Контейнер для модели, если нужен
            assetLoaderOptions: options // Передаем настройки
        );
    }






    private void OnDestroy()
    {
        foreach (string filePath in savedFiles)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Файл удален: {filePath}");
            }
        }
    }
}
