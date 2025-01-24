using System.Diagnostics;
using System.IO;
using UnityEngine;
using GLTFast;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;
using System.Collections;

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
                Arguments = $"\"{fbxPath}\" -o \"{outputPath}\"",
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

            Destroy(loadedModel);
            tempModel.transform.SetParent(slot.transform);
            tempModel.transform.localPosition = new Vector3(-0.6f, -0.5f, 0);
            tempModel.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);

            Debug.Log("Модель успешно загружена и добавлена в слот.");
        }
        else
        {
            Debug.LogError("Ошибка загрузки GLTF файла.");
        }
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
