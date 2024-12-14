using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                AudioSource audioSource = hit.collider.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    TogglePlayback(audioSource);
                }
            }
        }
    }

    public void TogglePlayback(AudioSource targetAudioSource)
    {
        if(gameObject.name == targetAudioSource.name) { 
            if (targetAudioSource.isPlaying) targetAudioSource.Pause();
            else targetAudioSource.Play();
        }
    }
}
