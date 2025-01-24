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

    private List<string> savedFiles = new List<string>();

    public void SetWorksInRoom(RoomWorksResponse roomWorksResponseIn)
    {
        
        Debug.LogError("set works in room");
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
                    LoadModelWithMaterials(workSlots[i], filePath);
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

            string savePath = Path.Combine(Application.persistentDataPath, fileName);

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

    private void LoadModelWithMaterials(GameObject slot, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"���� �� ������: {filePath}");
            return;
        }

        AssimpContext importer = new AssimpContext();
        Debug.Log($"������� �������� ������ ��: {filePath}");

        try
        {
            // ����������� �����
            Scene model = importer.ImportFile(filePath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

            Debug.Log($"���������� ���������� � ������: {model.MaterialCount}");

            // ������� ��� �������� �������
            Dictionary<string, Texture2D> loadedTextures = new Dictionary<string, Texture2D>();

            // �������� �� ���� �����
            foreach (var mesh in model.Meshes)
            {
                Debug.Log($"����������� ���: {mesh.Name} (�������� ������: {mesh.MaterialIndex})");

                // ������� ��� Unity
                UnityEngine.Mesh unityMesh = new UnityEngine.Mesh
                {
                    name = mesh.Name,
                    vertices = mesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(),
                    normals = mesh.Normals.Select(n => new Vector3(n.X, n.Y, n.Z)).ToArray(),
                    triangles = mesh.GetIndices()
                };
                
                unityMesh.RecalculateNormals();
               /* if (mesh.HasTextureCoords(0))
                {
                    unityMesh.uv = mesh.TextureCoordinateChannels[1].Select(uv => new Vector2(uv.X, uv.Y)).ToArray();
                }*/

                // ������� ������ ��� ����
                GameObject meshObject = new GameObject(mesh.Name);
                meshObject.transform.SetParent(slot.transform);

                // ��������� �������������
                meshObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
                meshObject.transform.localRotation = UnityEngine.Quaternion.Euler(0, 0, 0);
                meshObject.transform.localPosition = new Vector3(0, -0.55f, 0);

                // ��������� MeshFilter � MeshRenderer
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                meshFilter.mesh = unityMesh;

                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

                // �������� � ����������
                Assimp.Material material = model.Materials[mesh.MaterialIndex];
                UnityEngine.Material unityMaterial = new UnityEngine.Material(Shader.Find("Standard"));

                // ��������� ��������
                if (material.HasTextureDiffuse)
                {
                    TextureSlot textureSlot;
                    if (material.GetMaterialTexture(TextureType.Diffuse, 0, out textureSlot))
                    {
                        string texturePath = textureSlot.FilePath;

                        // ���������, ���������� ��� ������� ��������
                        Texture2D texture;
                        if (!string.IsNullOrEmpty(texturePath) && texturePath.StartsWith("*"))
                        {
                            // ���������� ��������
                            texture = LoadEmbeddedTexture(model, texturePath);
                        }
                        else
                        {
                            // ������� ��������
                            string fullPath = Path.Combine(Path.GetDirectoryName(filePath), texturePath);
                            if (!loadedTextures.TryGetValue(fullPath, out texture))
                            {
                                texture = LoadExternalTexture(fullPath);
                                loadedTextures[fullPath] = texture;
                            }
                        }

                        // ��������� �������� � ���������
                        if (texture != null)
                        {
                            unityMaterial.mainTexture = texture;
                        }
                    }
                }

                // ��������� ��������
                meshRenderer.material = unityMaterial;

                Debug.Log($"��� {mesh.Name} ������� ��������.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"������ ��� �������� FBX: {ex.Message}");
        }
    }

    // ����� ��� �������� ���������� ��������
    private Texture2D LoadEmbeddedTexture(Scene model, string texturePath)
    {
        int textureIndex = int.Parse(texturePath.Substring(1)); // ���������� ������ '*'
        if (textureIndex >= 0 && textureIndex < model.Textures.Count)
        {
            var embeddedTexture = model.Textures[textureIndex];
            int width = embeddedTexture.Width;
            int height = embeddedTexture.Height;

            Debug.Log($"���������� �������� {textureIndex}: ������ = {width}, ������ = {height}");
            
            if (embeddedTexture.HasCompressedData)
            {
                byte[] textureData = embeddedTexture.CompressedData;
                string savePath = Path.Combine(Application.persistentDataPath, "fileName.png");

                Texture2D texture = new Texture2D(1024, 1024);
                
                texture.LoadImage(textureData);
                texture.mipMapBias = 0;
                Debug.Log("������� ��������� ���������� ��������.");

                File.WriteAllBytes(savePath, textureData);
                return texture;
            }
        }

        Debug.LogError("�� ������� ��������� ���������� ��������.");
        return null;
    }

    // ����� ��� �������� ������� ��������
    private Texture2D LoadExternalTexture(string texturePath)
    {
        if (File.Exists(texturePath))
        {
            byte[] textureData = File.ReadAllBytes(texturePath);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(textureData);
            Debug.Log($"������� ��������� ������� �������� �� {texturePath}");
            return texture;
        }

        Debug.LogError($"�������� �� �������: {texturePath}");
        return null;
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
}
