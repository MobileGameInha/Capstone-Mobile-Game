using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TitleSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip baseButtonClickClip;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }


    public void PlayBaseButtonClick() { audioSource.PlayOneShot(baseButtonClickClip); }
}
