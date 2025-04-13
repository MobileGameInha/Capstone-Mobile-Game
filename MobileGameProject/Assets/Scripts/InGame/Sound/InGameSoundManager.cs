using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clickTileButtonClip;

    public AudioClip readyClip;

    public AudioClip startClip;

    public AudioClip footStepClip;

    public void PlayClickTileButtonClip() {
        audioSource.PlayOneShot(clickTileButtonClip);
    }

    public void PlayStartAnim_ReadyClip()
    {
        audioSource.PlayOneShot(readyClip);
    }

    public void PlayStartAnim_StartClip()
    {
        audioSource.PlayOneShot(startClip);
    }

    public void PlayStartAnim_FootStepClip()
    {
        audioSource.PlayOneShot(footStepClip);
    }
}


