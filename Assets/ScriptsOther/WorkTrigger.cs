using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkTrigger : MonoBehaviour
{
    private GameObject workListUI;
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
                    workListUI = obj.transform.GetChild(3).gameObject;
                    workDataUI = obj.transform.GetChild(4).gameObject;
                    inGameUI = obj.transform.GetChild(7).gameObject;
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
            workListUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInTrigger)
        { 
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OpenWorkList();
            } else if (Input.GetKeyDown(KeyCode.E))
            {
                OpenWorkData();
            }
        }
    }

    private void OpenWorkList()
    {
        workListUI.SetActive(true);
        workDataUI.SetActive(false);
        inGameUI.SetActive(false);
    }

    private void OpenWorkData()
    {
        workDataUI.SetActive(true);
        workListUI.SetActive(false);
        inGameUI.SetActive(false);
    }
}
