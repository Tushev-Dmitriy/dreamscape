using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicUI : MonoBehaviour
{
    //public GameObject audioControlPanel;
    //public Button playPauseButton;
    //public Slider volumeSlider;
    //public AudioSource targetAudioSource;

    private void Start()
    {
        //audioControlPanel.SetActive(false);
        //if (playPauseButton != null) playPauseButton.onClick.AddListener(TogglePlayback);
        //if (volumeSlider != null) volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                AudioSource audioSource = hit.collider.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    TogglePlayback(audioSource);
                    //OpenAudioControl(audioSource);
                }
            }
        }
    }

    public void OpenAudioControl(AudioSource source)
    {
        //targetAudioSource = source;
        //if (volumeSlider != null) volumeSlider.value = source.volume;
        //audioControlPanel.SetActive(true);
    }

    public void CloseAudioControl()
    {
        //audioControlPanel.SetActive(false);
    }

    public void TogglePlayback(AudioSource targetAudioSource)
    {
        if(gameObject.name == targetAudioSource.name) { 
            if (targetAudioSource.isPlaying) targetAudioSource.Pause();
            else targetAudioSource.Play();
        }
    }

    public void SetVolume(float value)
    {
        //if (targetAudioSource == null) return;
        //targetAudioSource.volume = value;
    }
}
