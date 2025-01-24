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
            Debug.LogError($"FBX ���� �� ������: {fbxPath}");
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
            Debug.LogError($"������ ��������� FBX � GLTF: {process.StandardError.ReadToEnd()}");
            return null;
        }

        string gltfPath = Path.Combine(outputPath + "_out", outputFileName + ".gltf");

        if (File.Exists(gltfPath))
        {
            Debug.Log($"����������� ��������� �������: {gltfPath}");
            return gltfPath;
        }
        else
        {
            Debug.LogError("GLTF ���� �� ������ ����� ���������!");
            return null;
        }
    }

    private async Task LoadGLTFToSlotAsync(string gltfPath, GameObject slot)
    {
        if (!File.Exists(gltfPath))
        {
            Debug.LogError($"GLTF ���� �� ������: {gltfPath}");
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

            Debug.Log("������ ������� ��������� � ��������� � ����.");
        }
        else
        {
            Debug.LogError("������ �������� GLTF �����.");
        }
    }

    private IEnumerator LoadGLTFToSlotCoroutine(string gltfPath, GameObject slot)
    {
        var task = LoadGLTFToSlotAsync(gltfPath, slot);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"������ ��� ��������: {task.Exception.Message}");
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
