using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RoomController : MonoBehaviour
{
    [HideInInspector] public RoomWorksResponse roomWorksResponse;
    public List<GameObject> workSlots;

    public void SetWorksInRoom(RoomWorksResponse roomWorksResponseIn)
    {
        roomWorksResponse = roomWorksResponseIn;

        List<Work> allWorkInRoom = roomWorksResponse.Works;

        for (int i = 0; i < allWorkInRoom.Count; i++)
        {
            Work tempWork = allWorkInRoom[i];
            if (tempWork.WorkID != -1)
            {
                //string typeOfWork = tempWork.WorkType;
                //switch (typeOfWork)
                //{
                //    case "Image":
                //        break;
                //    case "Music":
                //        break;
                //    case "Model":
                //        break;
                //}
                Material tempImgMaterial = new Material(Shader.Find("Standart"));
                //tempImgMaterial.SetTexture("MainTex", )

                string filepath = SaveWorkToFile(tempWork);

                workSlots[i].transform.GetChild(1).GetComponent<MeshRenderer>();
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

            Debug.Log($"Файл {fileName} успешно сохранен в {savePath}");

            return savePath;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка сохранения файла для работы {work.WorkID}: {ex.Message}");
            return null;
        }
    }
}
