using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityGLTF;
using Debug = UnityEngine.Debug;
using Unity.VisualScripting;
using GLTF;
using System.Threading;

public class FBXToGLTFConverter : MonoBehaviour
{
    [Header("Paths")]
    private string fbxFilePath;
    private string fbx2glTFPath;

    private string gltfFilePath;

    GameObject tempGo;
    GameObject tempSlot;

    private void Start()
    {
        fbx2glTFPath = $"{Application.streamingAssetsPath}/FBX2glTF-windows-x64.exe";
    }

    private void ConvertFBXToGLTF(string fbxPath)
    {
        tempGo = null;

        if (!File.Exists(fbxPath))
        {
            Debug.LogError($"FBX файл не найден: {fbxPath}");
            return;
        }

        string outputFileName = Path.GetFileNameWithoutExtension(fbxPath);

        string outputDirectory = Path.GetDirectoryName(fbxPath);
        string outputPath = Path.Combine(outputDirectory, outputFileName);

        Process process = new Process();
        process.StartInfo.FileName = fbx2glTFPath;
        process.StartInfo.Arguments = $"\"{fbxPath}\" -o \"{outputPath}\"";
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();
        process.WaitForExit();

        string error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0)
        {
            Debug.LogError($"Ошибка при конверсии FBX в GLTF: {error}");
            return;
        }

        gltfFilePath = Path.Combine(outputPath + "_out", outputFileName + ".gltf");

        if (File.Exists(gltfFilePath))
        {
            Debug.Log($"Конвертация завершена успешно: {gltfFilePath}");
        }
        else
        {
            Debug.LogError("GLTF файл не найден после конверсии!");
        }
    }

    private IEnumerator LoadGLTFInScene(GameObject slot, string gltfPath)
    {
        tempSlot = null;

        if (!System.IO.File.Exists(gltfPath))
        {
            Debug.LogError($"GLTF файл не найден: {gltfPath}");
            yield break;
        }

        string localFileUri = "file://" + gltfPath;

        ImportOptions importOptions = new ImportOptions
        {
            AsyncCoroutineHelper = gameObject.GetOrAddComponent<AsyncCoroutineHelper>()
        };

        GLTFSceneImporter importer = new GLTFSceneImporter(localFileUri, importOptions);

        tempSlot = slot;

        yield return importer.LoadSceneAsync(-1);
    }

    public void SetupPosForModels()
    {
        GameObject tempGo = GameObject.Find("Root Scene").transform.GetChild(0).GetChild(0).gameObject;
        tempGo.transform.SetParent(tempSlot.transform);
        tempGo.transform.localPosition = new Vector3(-0.45f, -0.5f, 0);
        tempGo.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    public void ConvertAndSpawnModel(GameObject slot, string path)
    {
        fbxFilePath = path;
        ConvertFBXToGLTF(fbxFilePath);
        if (!string.IsNullOrEmpty(gltfFilePath))
        {
            StartCoroutine(LoadGLTFInScene(slot, gltfFilePath));
        }
    }
}
