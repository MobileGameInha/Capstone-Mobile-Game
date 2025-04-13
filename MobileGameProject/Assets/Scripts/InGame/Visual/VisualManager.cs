using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
    public GameStartEffect startEffect;
    public InGameSoundManager soundManager;

    public GameObject IdleCatPack;
    public RectTransform MovingCatPackTransform;

    public SkeletonAnimation[] IdleCats = new SkeletonAnimation[3];
    public SkeletonAnimation[] MovingCats = new SkeletonAnimation[3];

    private bool cats_is_moving_;

    private float cats_speed_ = 1.25f;
    private float cat_footstep_timer_;
    private float cat_footstep_timer_max_ = 0.5f;
    private bool start_trigger_ = false;

    public Image Co2FillImage;
    public Slider Co2Slider;
    public float Co2Speed =1.0f;

    public Color Co2MinColor;
    public Color Co2MaxColor;

    private bool co2_is_chaging_;
    private float co2_rate_;
    private float co2_rate_destination_;

    private void Awake()
    {
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
        for (int i = 0; i < cats.Length; i++)
        {
            if (cats[i] <= 0 || cats[i] >= GameManager.CAT_SIZE_)
            {
                IdleCats[i].gameObject.SetActive(false);
                MovingCats[i].gameObject.SetActive(false);
            }
            else
            {
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
        cat_footstep_timer_ = cat_footstep_timer_max_;
    }


}
