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

    public AudioClip[] disruptorCreatureClips;
    public AudioClip[] disruptorClip;

    public AudioClip[] catSkillClip;

    public AudioClip CrowClip;

    public void PlayCrowClip()
    {
        audioSource.PlayOneShot(CrowClip);
    }

    public void PlayCatSkillClips(int idx, bool playSkillSound) 
    {
        if (idx >= 0 && idx < GameManager.CAT_SIZE_)
        {
            if (playSkillSound) { 
                audioSource.PlayOneShot(catSkillClip[idx]);
            }
        }
    }

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

    public void PlayDisruptureClip(int idx) 
    {
        audioSource.PlayOneShot(disruptorClip[idx]);
        audioSource.PlayOneShot(disruptorCreatureClips[idx]);
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


