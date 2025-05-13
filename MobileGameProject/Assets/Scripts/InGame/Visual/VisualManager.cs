using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    private enum EffectDeleteType { NONE, TIMER, NEXT_ROUND}

    private readonly int SHOW_PARAM_HASH = Animator.StringToHash("SHOW");

    private static readonly int[] CAT_INDEX_GOOD = { 1, 7, 8, 15, 17, 18, 20, 22, 23, 25, 26 };
    private static readonly int[] CAT_INDEX_BAD = { 2,3,4,5,6,9,10,12,13,16,21,24 };
    private static readonly int CAT_INDEX_FEVER = 19;
    private static readonly int CAT_INDEX_SKILL = 11;

    private static readonly float[] CAT_SKILL_TIMER = 
        { 3.0f, 3.0f, 3.0f, 3.0f, 3.0f,
    3.0f,1.5f,3.0f,3.0f,3.0f,
    3.0f,3.0f,
    };

    private EffectDeleteType[] CAT_EFFECT_DELETE_TYPE_ =
        { EffectDeleteType.TIMER, EffectDeleteType.TIMER, EffectDeleteType.TIMER, EffectDeleteType.TIMER, EffectDeleteType.NONE
    ,EffectDeleteType.NONE, EffectDeleteType.TIMER, EffectDeleteType.NEXT_ROUND, EffectDeleteType.NEXT_ROUND, EffectDeleteType.NEXT_ROUND
    ,EffectDeleteType.NEXT_ROUND, EffectDeleteType.NEXT_ROUND,
    };

    public GameStartEffect startEffect;
    public InGameSoundManager soundManager;

    public GameObject IdleCatPack;
    public RectTransform MovingCatPackTransform;

    public SkeletonAnimation[] IdleCats = new SkeletonAnimation[3];
    public SkeletonAnimation[] MovingCats = new SkeletonAnimation[3];

    private bool cats_is_moving_;

    private float cats_speed_ = 1.75f;
    private float cat_footstep_timer_;
    private float cat_footstep_timer_max_ = 0.5f;
    private bool start_trigger_ = false;
    private bool end_trigger_ = false;

    public Image Co2FillImage;
    public Slider Co2Slider;
    public float Co2Speed =1.0f;

    public Color Co2MinColor;
    public Color Co2MaxColor;

    private bool co2_is_chaging_;
    private float co2_rate_;
    private float co2_rate_destination_;

    public InGameSoundManager inGameSoundManager;

    public Animator FeverAnimator;
    public ParticleSystem FeverOnEffect;

    public Animator[] DisructorAnimators;

    public GameObject[] CatSkillEffect = new GameObject[BasicHelperManager.MAX_HELPER_];
    public GameObject[] SkillEffect = new GameObject[GameManager.CAT_SIZE_];


    private int[] cat_index_ = new int[3];
    private bool[] is_playing_cat_effect_ = new bool[3];
    private float[] cat_effect_timer_ = new float[3];


    public GameObject[] SimpleLineEffects;


    public Animator HideAnimator;
    public Animator EndAnimator;

    public TMP_Text ScoreText;
    public TMP_Text CoinText;
    public TMP_Text EXPText;

    private void Awake()
    {
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            cat_index_[i] = -1;
            is_playing_cat_effect_[i] = false;
        }

        for (int i = 0; i < CatSkillEffect.Length; i++)
        {
            CatSkillEffect[i].SetActive(false);
        }

        for (int i = 0; i < SkillEffect.Length; i++)
        {
            if(SkillEffect[i]!=null)
                SkillEffect[i].SetActive(false);
        }

        start_trigger_ = false;

        cats_is_moving_ = false;
        IdleCatPack.SetActive(false);
        MovingCatPackTransform.gameObject.SetActive(true);

        co2_is_chaging_ = false;
        co2_rate_ = 0.5f;
        ChangeCo2Visual();
    }

    private void Update()
    {
        if (co2_is_chaging_)
        {
            if (co2_rate_ > co2_rate_destination_)
            {
                co2_rate_ -= Time.deltaTime * Co2Speed;
                if (co2_rate_ <= co2_rate_destination_) { co2_rate_ = co2_rate_destination_; co2_is_chaging_ = false; }
            }
            else
            {
                co2_rate_ += Time.deltaTime * Co2Speed;
                if (co2_rate_ >= co2_rate_destination_) { co2_rate_ = co2_rate_destination_; co2_is_chaging_ = false; }
            }

            ChangeCo2Visual();
        }

        if (cats_is_moving_)
        {
            float x = MovingCatPackTransform.position.x + Time.deltaTime * cats_speed_;

            cat_footstep_timer_ -= Time.deltaTime;
            if(cat_footstep_timer_ <= 0.0f)
            {
                soundManager.PlayStartAnim_FootStepClip();
                cat_footstep_timer_ = cat_footstep_timer_max_;
            }

            if (x >= 0.0f) {
                x = 0.0f;
            }
            MovingCatPackTransform.position = new Vector3(x, MovingCatPackTransform.position.y, MovingCatPackTransform.position.z);
            if (x == 0.0f) 
            {
                cats_is_moving_ = false;
                IdleCatPack.SetActive(true);
                MovingCatPackTransform.gameObject.SetActive(false);
                startEffect.StartAnim();
            }
        }
    }

    public void ResetCatSkill()
    {
        for (int i = 0; i < SkillEffect.Length; i++)
        {
            if (CAT_EFFECT_DELETE_TYPE_[i] == EffectDeleteType.NEXT_ROUND)
            {
                SkillEffect[i].SetActive(false);
            }
        }

        for (int j = 0; j < CatSkillEffect.Length; j++)
        {
            if (cat_index_[j] != -1 && CAT_EFFECT_DELETE_TYPE_[cat_index_[j]] == EffectDeleteType.NEXT_ROUND)
            {
                CatSkillEffect[j].SetActive(false);
            }
        }

    }

    public void PlayCatSkill(int idx, bool playSkillSound = true, int simplelineIndex = 0)
    {
        int index = -1;

        Debug.Log(idx + "고양이 스킬 사용!");

        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            if (cat_index_[i] == idx) { index = i; }
        }

        if (index > -1 && index < 3) 
        {
            if (is_playing_cat_effect_[index])
            {
                Debug.Log("이미 타이머 진입 상태!");
                inGameSoundManager.PlayCatSkillClips(index, playSkillSound);
                SetCatAnimationSkill(index);
                cat_effect_timer_[index] += CAT_SKILL_TIMER[idx];
            }
            else 
            {
                if (CAT_EFFECT_DELETE_TYPE_[idx] == EffectDeleteType.NEXT_ROUND)
                {
                    Debug.Log(index+" : 넥스트 라운드 타입 발동!");
                    inGameSoundManager.PlayCatSkillClips(index, playSkillSound);
                    CatSkillEffect[index].SetActive(true);
                    if (SkillEffect[cat_index_[index]] != null)
                    {
                        SkillEffect[cat_index_[index]].SetActive(true);
                    }
                    if (idx == (int)CatIndex.SIMPLE_LINE_)
                    {
                        for (int i = 0; i < SimpleLineEffects.Length; i++)
                        {
                            if (i == simplelineIndex) { SimpleLineEffects[i].SetActive(true); }
                            else { SimpleLineEffects[i].SetActive(false); }
                        }
                    }

                }
                else if (CAT_EFFECT_DELETE_TYPE_[idx] == EffectDeleteType.TIMER)
                {
                    Debug.Log("타이머 타입 발동!");
                    inGameSoundManager.PlayCatSkillClips(index, playSkillSound);
                    SetCatAnimationSkill(index);
                    cat_effect_timer_[index] = CAT_SKILL_TIMER[idx];
                    is_playing_cat_effect_[index] = true;
                    StartCoroutine(CatSkillCoroutine(index));
                }
            }
        }

    }

    private IEnumerator CatSkillCoroutine(int idx) {
        CatSkillEffect[idx].SetActive(true);
        if (SkillEffect[cat_index_[idx]] != null)
        {
            SkillEffect[cat_index_[idx]].SetActive(true);
        }

        while (cat_effect_timer_[idx] > 0.0f) 
        {
            cat_effect_timer_[idx] -= CAT_SKILL_TIMER[cat_index_[idx]];
            yield return new WaitForSeconds(CAT_SKILL_TIMER[cat_index_[idx]]);
        }

        CatSkillEffect[idx].SetActive(false);
        if (SkillEffect[cat_index_[idx]] != null)
        {
            SkillEffect[cat_index_[idx]].SetActive(false);
        }

        is_playing_cat_effect_[idx] = false;
    }

    public void ShowDisruptor(int idx) 
    {
        if (DisructorAnimators[idx] != null) {
            DisructorAnimators[idx].SetTrigger(SHOW_PARAM_HASH);
            inGameSoundManager.PlayDisruptureClip(idx);
        }
    }

    public void StartFever()
    {
        inGameSoundManager.PlayFeverClip();
        FeverAnimator.SetBool(SHOW_PARAM_HASH, true);
        FeverOnEffect.Play();
    }

    public void EndFever()
    {
        FeverAnimator.SetBool(SHOW_PARAM_HASH, false);
    }

    public void SetCatAnimation(bool isGood) 
    {
        if (isGood)
        {
            inGameSoundManager.PlayCatClips(true);
            for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
            {
                if (cat_index_[i]!=-1 &&IdleCats[i] != null)
                    IdleCats[i].AnimationState.SetAnimation(0, "idle-" + CAT_INDEX_GOOD[(Random.Range(0, CAT_INDEX_GOOD.Length))].ToString(), true);
            }
        }
        else {
            inGameSoundManager.PlayCatClips(false);
            for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
            {
                if (cat_index_[i] != -1 && IdleCats[i] != null)
                    IdleCats[i].AnimationState.SetAnimation(0, "idle-" + CAT_INDEX_BAD[(Random.Range(0, CAT_INDEX_BAD.Length))].ToString(), true);
            }
        }
    }

    public void SetCatAnimationSkill(int cat_index)
    {
         if (cat_index_[cat_index] != -1 && IdleCats[cat_index] != null)
             IdleCats[cat_index].AnimationState.SetAnimation(0, "idle-" + CAT_INDEX_SKILL.ToString(), true);
    }

    public void SetCatAnimationFever()
    {
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            if (cat_index_[i] != -1 && IdleCats[i] != null)
                IdleCats[i].AnimationState.SetAnimation(0, "idle-" + CAT_INDEX_SKILL.ToString(), true);
        }
    }

    public void SetCo2Value(float value) {
        co2_is_chaging_ = true;
        co2_rate_destination_ = value;
    }

    private void ChangeCo2Visual() {
        Co2FillImage.color = Color.Lerp(Co2MinColor, Co2MaxColor, co2_rate_);
        Co2Slider.value = co2_rate_;
    }

    public void SetCatState(int[] cats)
    {
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            cat_index_[i] = -1;
        }

        for (int i = 0; i < cats.Length; i++)
        {
            if (cats[i] < 0 || cats[i] >= GameManager.CAT_SIZE_)
            {
                IdleCats[i].gameObject.SetActive(false);
                MovingCats[i].gameObject.SetActive(false);
            }
            else
            {
                cat_index_[i] = cats[i];
                MovingCats[i].gameObject.SetActive(true);
                MovingCats[i].initialSkinName = "Cat-" + (cats[i] + 1).ToString();
                MovingCats[i].Initialize(true);
                IdleCats[i].gameObject.SetActive(true);
                IdleCats[i].initialSkinName = "Cat-" + (cats[i] + 1).ToString();
                IdleCats[i].Initialize(true);
            }
        }
    }

    public void StartAnimationForStartGame() {
        if (start_trigger_) { return; }
        cats_is_moving_ = true;
        start_trigger_ = true;
        bool hasCat = false;
        for (int i = 0; i < cat_index_.Length; i++)
        {
            if (cat_index_[i] != -1) {
                hasCat = true;
                break;
            }
        }

        if (hasCat)
        {
            cat_footstep_timer_ = cat_footstep_timer_max_;
        }
        else {
            cat_footstep_timer_ = 500000.0f;
            inGameSoundManager.PlayCrowClip();
        }
 
    }

    public void StartAnimationForEndGame(int score, int coin, int exp, bool coin_up, bool exp_up)
    {
        if (end_trigger_) { return; }
        end_trigger_ = true;

        ScoreText.text = score.ToString();
        CoinText.text = coin.ToString();
        EXPText.text = exp.ToString();

        if (coin_up) { CoinText.color = Color.red; } else { CoinText.color = Color.white; }
        if (exp_up) { EXPText.color = Color.red; } else { EXPText.color = Color.white; }

        inGameSoundManager.PlayStartAnim_EndClip();
        GameObject.FindObjectOfType<InGameBGMManager>().EndBGM();
        HideAnimator.SetBool(SHOW_PARAM_HASH, true);
        StartCoroutine(StartAnimationForEndGameCoroutine());
    }

    private IEnumerator StartAnimationForEndGameCoroutine() {
        yield return new WaitForSeconds(2f);
        EndAnimator.SetBool(SHOW_PARAM_HASH, true);
    }
}
