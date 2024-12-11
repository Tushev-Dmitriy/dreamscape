using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    [SerializeField] private FadeChannelSO _fadeChannelSO;
    [SerializeField] private Image _imageComponent;

    private void OnEnable()
    {
        _fadeChannelSO.OnEventRaised += InitiateFade;
    }

    private void OnDisable()
    {
        _fadeChannelSO.OnEventRaised -= InitiateFade;
    }
    
    private void InitiateFade(bool fadeIn, float duration, Color desiredColor)
    {
        _imageComponent.DOBlendableColor(desiredColor, duration);
    }
}
