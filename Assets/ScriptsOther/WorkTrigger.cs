using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkTrigger : MonoBehaviour
{
    private GameObject workDataUI;
    private GameObject inGameUI;

    private bool playerInTrigger;

    private void Start()
    {
        Scene managerScene = SceneManager.GetSceneByName("ManagerScene");
        if (managerScene.IsValid() && managerScene.isLoaded)
        {
            foreach (GameObject obj in managerScene.GetRootGameObjects())
            {
                if (obj.name == "Canvas")
                {
                    workDataUI = obj.transform.GetChild(3).gameObject;
                    inGameUI = obj.transform.GetChild(6).gameObject;
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            inGameUI.SetActive(true);
            workDataUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            OpenWorkData();
        }
    }

    private void OpenWorkData()
    {
        workDataUI.SetActive(true);
        inGameUI.SetActive(false);
    }
}
