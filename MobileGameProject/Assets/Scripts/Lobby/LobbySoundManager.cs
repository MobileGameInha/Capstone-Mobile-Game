using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LobbySoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip quickButtonClickClip;

    public AudioClip baseButtonClickClip;


    public AudioClip stageButtonClickClip;
    public AudioClip stageSweepButtonClickClip;
    public AudioClip stageReturnButtonClickClip;
    public AudioClip stageStartButtonClickClip;

    public AudioClip[] catClips;

    public AudioClip challangeChangeButtonClickClip;

    public AudioClip shopMenuButtonClickClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }



    public void PlayQuickButtonClick() { audioSource.PlayOneShot(quickButtonClickClip); }

    public void PlayBaseButtonClick() { audioSource.PlayOneShot(baseButtonClickClip); }

    public void PlayStageButtonClick() { audioSource.PlayOneShot(stageButtonClickClip); }
    public void PlayStageSweepButtonClick() { audioSource.PlayOneShot(stageSweepButtonClickClip); }
    public void PlayStageReturnButtonClick() { audioSource.PlayOneShot(stageReturnButtonClickClip); }
    public void PlayStageStartButtonClick() { audioSource.PlayOneShot(stageStartButtonClickClip); }


    public void PlayCatClick() { 
        audioSource.PlayOneShot(catClips[Random.Range(0, catClips.Length)]); 
    }


    public void PlayChallangeChangeButtonClickClip() { audioSource.PlayOneShot(challangeChangeButtonClickClip); }

    public void PlayShopMenuButtonClickClip() { audioSource.PlayOneShot(shopMenuButtonClickClip); }
}
