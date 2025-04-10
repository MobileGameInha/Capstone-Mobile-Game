using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip clickTileButtonClip;

    public void PlayClickTileButtonClip() {
        audioSource.PlayOneShot(clickTileButtonClip);
    }
}

