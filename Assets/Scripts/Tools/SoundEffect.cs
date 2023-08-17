using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GlobalController globalController;
        try
        {
            globalController = GlobalController.Instance;
            SetSound(globalController.gameOptions);
        }
        catch
        {
            Debug.LogWarning("setSoundFailed");
        }
    }

    void SetSound(GameOptions options)
    {
        _audioSource.mute = options.soundSettings[2] == 0;
    }

}
