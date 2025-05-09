using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartEffect : MonoBehaviour
{
    private readonly int SHOW_PARAM_HASH = Animator.StringToHash("SHOW");

    public Animator animator;

    public InGameSoundManager inGameSoundManager;
    public void StartAnim() {
        animator.SetTrigger(SHOW_PARAM_HASH);
    }

    public void EndAnim()
    {
        GameManager.gameManager.StartGame();
    }

    public void PlaySoundReady()
    {
        inGameSoundManager.PlayStartAnim_ReadyClip();
    }
    public void PlaySoundStart()
    {
        inGameSoundManager.PlayStartAnim_StartClip();
    }
}
