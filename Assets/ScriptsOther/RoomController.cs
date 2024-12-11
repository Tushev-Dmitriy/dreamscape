using System;
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
    
    private static RoomController _instance;

    public static RoomController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<RoomController>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        foreach (string filePath in savedFiles)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"���� ������: {filePath}");
            }
        }
    }

    public void SetWorksInRoom(RoomWorksResponse roomWorksResponseIn)
    {
        roomWorksResponse = roomWorksResponseIn;

        List<Work> allWorkInRoom = roomWorksResponse.Works;

        for (int i = 0; i < workSlots.Count; i++)
        {
            // ���������, ���� �� ������ ��� �������� �����
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
                    Debug.LogWarning($"����������� ��� ������: {tempWork.WorkType}");
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

            savedFiles.Add(savePath); // ��������� ���� � ������ ��� ������������ ��������

            Debug.Log($"���� {fileName} ������� �������� � {savePath}");

            return savePath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"������ ���������� ����� ��� ������ {work.WorkID}: {ex.Message}");
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
            Debug.LogError($"�� ������� ��������� �������� �� �����: {filePath}");
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
                Debug.Log($"����� ������� ��������� �� {filePath}");
            }
            else
            {
                Debug.LogError($"������ �������� ����� �� {filePath}: {www.error}");
            }
        }
    }

    private void ClearSlot(GameObject slot)
    {
        // ������� ��������, ���� ����
        MeshRenderer renderer = slot.GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material = null;
        }

        // ������� ���������, ���� ����
        AudioSource audioSource = slot.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
    }

    private void LoadModelWithTriLib(string filePath, GameObject slot)
    {
        var options = AssetLoader.CreateDefaultLoaderOptions(); // ��������� ����������

        AssetLoader.LoadModelFromFile(
            filePath,
            onLoad: (context) =>
            {
                GameObject loadedGameObject = context.RootGameObject; // �������� ����������� ������
                if (loadedGameObject != null)
                {
                    loadedGameObject.transform.SetParent(slot.transform);
                    loadedGameObject.transform.localPosition = new Vector3(0, -0.5f, 0); // ������� �� y -0.5
                    loadedGameObject.transform.localRotation = Quaternion.Euler(0, -90, 0); // ������� �� y �� -90 ��������
                    loadedGameObject.transform.localScale = new Vector3(3, 3, 3); // ������� 3x3x3

                    Debug.Log($"������ ������� ��������� � ����: {slot.name}");
                }
            },
            onMaterialsLoad: null, // ���������� ����� �������� ����������
            onProgress: (context, progress) =>
            {
                Debug.Log($"�������� �������� ������: {progress * 100:F2}%");
            },
            onError: (error) =>
            {
                Debug.LogError($"������ �������� ������: {error}");
            },
            wrapperGameObject: null, // ��������� ��� ������, ���� �����
            assetLoaderOptions: options // �������� ���������
        );
    }
}
