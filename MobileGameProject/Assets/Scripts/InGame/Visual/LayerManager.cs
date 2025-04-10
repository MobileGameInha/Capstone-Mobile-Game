using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerManager : MonoBehaviour
{
    private readonly int SHOW_PARAM_HASH= Animator.StringToHash("SHOW");

    public Animator[] layerAnimators;

    private int layer_count_;
    private bool[] is_showing_;

    public Image blackPanel;


    private void Awake()
    {
        layer_count_ = layerAnimators.Length;
        is_showing_ = new bool[layer_count_];

        blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, 50.0f / 255.0f);

        for (int i = 0; i < layer_count_; i++)
        {
            layerAnimators[i].SetBool(SHOW_PARAM_HASH, false);
            is_showing_[i] = false;
        }
    }


    public void SetLayer(float life_rate) {

        blackPanel.color = new Color(blackPanel.color.r, blackPanel.color.g, blackPanel.color.b, (100.0f*life_rate) / 255.0f);

        for (int i = 0; i < layer_count_; i++)
        {

            if (((float)(i+1) / (float)layer_count_) <= life_rate)
            {
                if (is_showing_[i])
                {
                    is_showing_[i] = false;
                    layerAnimators[i].SetBool(SHOW_PARAM_HASH, false);
                }
            }
            else 
            {
                if (!is_showing_[i])
                {
                    is_showing_[i] = true;
                    layerAnimators[i].SetBool(SHOW_PARAM_HASH, true);
                }
            }
        }
    }
}
