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

    public AudioClip failClip;

    public AudioClip feverClip;

    public AudioClip[] failCatClips;

    public AudioClip[] successCatClips;


    public void PlayCatClips(bool isSuccess)
    {
        if (isSuccess)
        {
            audioSource.PlayOneShot(successCatClips[Random.Range(0, successCatClips.Length)]);
        }
        else 
        {
            audioSource.PlayOneShot(failCatClips[Random.Range(0, failCatClips.Length)]);
        }
    }

    public void PlayFeverClip()
    {
        audioSource.PlayOneShot(feverClip);
    }

    public void PlayClickFailClip()
    {
        audioSource.PlayOneShot(failClip);
    }

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


