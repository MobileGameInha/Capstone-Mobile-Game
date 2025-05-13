using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicChallangeManager : MonoBehaviour
{
    private readonly int BW_PARAM_HASH = Animator.StringToHash("BW");
    private readonly int OL_PARAM_HASH = Animator.StringToHash("OL");
    private readonly int ROT_PARAM_HASH = Animator.StringToHash("ROT");
    public enum State {BW,OL,ROT};

    public Animator animator;

    public SkeletonAnimation[] Cats = new SkeletonAnimation[5];

    private State state_ = State.BW;
    private void Awake()
    {
        state_ = State.BW;

        animator.SetBool(OL_PARAM_HASH, false);
        animator.SetBool(ROT_PARAM_HASH, false);
        animator.SetBool(BW_PARAM_HASH, false);
    }

    private void Start()
    {
        ResetCatState();
    }

    public void OnClickChangeMode(int idx) {
        if (idx < 0 && idx > 2) { return; }
        switch ((State)idx)
        {
            case State.BW:
                state_ = State.BW;
                animator.SetBool(OL_PARAM_HASH,false);
                animator.SetBool(ROT_PARAM_HASH,false);
                animator.SetBool(BW_PARAM_HASH, true);
                break;
            case State.OL:
                state_ = State.OL;
                animator.SetBool(ROT_PARAM_HASH, false);
                animator.SetBool(BW_PARAM_HASH, false);
                animator.SetBool(OL_PARAM_HASH, true);
                break;
            case State.ROT:
                state_ = State.ROT;
                animator.SetBool(OL_PARAM_HASH, false);
                animator.SetBool(BW_PARAM_HASH, false);
                animator.SetBool(ROT_PARAM_HASH, true);
                break;
            default:
                break;
        }
    }

    public void OnClickStartButton()
    {
        switch (state_)
        {
            case State.BW:
                LoadingManager.LoadScene("BlackAndWhiteScene");
                break;
            case State.OL:
                LoadingManager.LoadScene("OneLifeScene");
                break;
            case State.ROT:
                LoadingManager.LoadScene("RotationScene");
                break;
            default:
                break;
        }
    }

    public void ResetCatState()
    {
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            if (DataManager.dataManager.GetSelectedCat(i) != -1)
            {
                Cats[i].gameObject.SetActive(true);
                Cats[i].initialSkinName = "Cat-" + (DataManager.dataManager.GetSelectedCat(i) + 1).ToString();
                Cats[i].Initialize(true);
            }
            else
            {
                Cats[i].gameObject.SetActive(false);
            }
        }

    }  

}
