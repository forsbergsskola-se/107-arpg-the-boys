using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NukeSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake()
    {
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
}
