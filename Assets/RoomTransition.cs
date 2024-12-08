using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    [Header("Objects to Transfer")]
    public Transform player;
    public Transform[] objectsToTransfer;

    [Header("Target Scene")]
    public string roomSceneName;

    public void TransitionToRoomScene()
    {
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        SaveObjectsState();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(roomSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        TransferObjects();
    }

    private void SaveObjectsState()
    {
        foreach (var obj in objectsToTransfer)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void TransferObjects()
    {
        foreach (var obj in objectsToTransfer)
        {
            obj.gameObject.SetActive(true);
        }

        player.position = new Vector3(0, 0, 0);
    }
}
