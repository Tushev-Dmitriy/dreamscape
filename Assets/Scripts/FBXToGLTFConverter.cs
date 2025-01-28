using System.Diagnostics;
using System.IO;
using UnityEngine;
using GLTFast;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class FBXToGLTFConverter : MonoBehaviour
{
    private string fbx2glTFPath;

    private void Start()
    {
        fbx2glTFPath = $"{Application.streamingAssetsPath}/FBX2glTF-windows-x64.exe";
    }

    private string ConvertFBXToGLTF(string fbxPath)
    {
        if (!File.Exists(fbxPath))
        {
            Debug.LogError($"FBX файл не найден: {fbxPath}");
            return null;
        }

        string outputFileName = Path.GetFileNameWithoutExtension(fbxPath);
        string outputDirectory = Path.GetDirectoryName(fbxPath);
        string outputPath = Path.Combine(outputDirectory, outputFileName);

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fbx2glTFPath,
                Arguments = $"\"{fbxPath}\" -o \"{outputPath}\" --embed",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            Debug.LogError($"Ошибка конверсии FBX в GLTF: {process.StandardError.ReadToEnd()}");
            return null;
        }

        string gltfPath = Path.Combine(outputPath + "_out", outputFileName + ".gltf");

        if (File.Exists(gltfPath))
        {
            Debug.Log($"Конвертация завершена успешно: {gltfPath}");
            return gltfPath;
        }
        else
        {
            Debug.LogError("GLTF файл не найден после конверсии!");
            return null;
        }
    }


    private async Task LoadGLTFToSlotAsync(string gltfPath, GameObject slot)
    {
        if (!File.Exists(gltfPath))
        {
            Debug.LogError($"GLTF файл не найден: {gltfPath}");
            return;
        }

        var gltfImport = new GltfImport();
        bool success = await gltfImport.Load(gltfPath);

        if (success && gltfImport != null && gltfImport.SceneCount > 0)
        {
            GameObject loadedModel = new GameObject("LoadedModel");
            gltfImport.InstantiateSceneAsync(loadedModel.transform);

            GameObject tempModel = loadedModel.transform.GetChild(0).GetChild(0).gameObject;

            //tempModel.GetComponent<Renderer>().material.shader = Shader.Find("glTF/PbrMetallicRoughness");

            Destroy(loadedModel);
            tempModel.transform.SetParent(slot.transform);
            FitModelToSlot(tempModel, slot);

            Debug.Log("Модель успешно загружена и добавлена в слот.");
        }
        else
        {
            Debug.LogError("Ошибка загрузки GLTF файла.");
        }
    }

    private void FitModelToSlot(GameObject model, GameObject slot)
    {
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        List<Renderer> renderers = model.GetComponentsInChildren<Renderer>().ToList();
        if (renderers.Count == 0)
        {
            renderers.Add(model.AddComponent<Renderer>());
        }

        Bounds modelBounds = new Bounds(renderers[0].bounds.center, renderers[0].bounds.size);
        foreach (var renderer in renderers)
        {
            modelBounds.Encapsulate(renderer.bounds);
        }

        BoxCollider slotCollider = slot.GetComponent<BoxCollider>();
        if (slotCollider == null)
        {
            Debug.LogError("Слот не имеет компонента BoxCollider для определения размеров.");
            return;
        }

        Bounds slotBounds = slotCollider.bounds;

        float scaleX = slotBounds.size.x / modelBounds.size.x;
        float scaleY = slotBounds.size.y / modelBounds.size.y;
        float scaleZ = slotBounds.size.z / modelBounds.size.z;

        float scaleFactor = Mathf.Min(scaleX, scaleY, scaleZ);

        model.transform.localScale = Vector3.one * scaleFactor;

        model.transform.localPosition = new Vector3(0, -0.05f, 0);
    }


    private IEnumerator LoadGLTFToSlotCoroutine(string gltfPath, GameObject slot)
    {
        var task = LoadGLTFToSlotAsync(gltfPath, slot);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Ошибка при загрузке: {task.Exception.Message}");
        }
    }

    public void ConvertAndAddModel(GameObject slot, string fbxPath)
    {
        string gltfPath = ConvertFBXToGLTF(fbxPath);
        if (!string.IsNullOrEmpty(gltfPath))
        {
            StartCoroutine(LoadGLTFToSlotCoroutine(gltfPath, slot));
        }
    }
}
