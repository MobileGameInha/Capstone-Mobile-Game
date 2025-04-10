using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualManager : MonoBehaviour
{
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
    }

    public void SetCo2Value(float value) {
        co2_is_chaging_ = true;
        co2_rate_destination_ = value;
    }

    private void ChangeCo2Visual() {
        Co2FillImage.color = Color.Lerp(Co2MinColor, Co2MaxColor, co2_rate_);
        Co2Slider.value = co2_rate_;
    }


}
