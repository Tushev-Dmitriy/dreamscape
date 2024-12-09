using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransition : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public bool isRoom = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            sceneLoader.SwitchScene(isRoom);
        }
    }
}
