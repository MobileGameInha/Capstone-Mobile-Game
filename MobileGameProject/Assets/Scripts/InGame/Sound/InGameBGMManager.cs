using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBGMManager : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip BGM;

    private void Awake()
    {
        audioSource.loop = true;
    }

    public void PlayBGM()
    {
        audioSource.clip = BGM;
        audioSource.Play();
    }

    public void EndBGM()
    {
        audioSource.Stop();
    }
}
