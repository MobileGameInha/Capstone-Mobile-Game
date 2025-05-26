using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject PanelPack;
    public GameObject[] Panels;

    private int index_ = 0;
    private bool is_open_ = false;

    private void Awake()
    {
        is_open_ = false;
    }

    public void Open() {
        if (is_open_) { return; }

        index_ = 0;
        is_open_ = true;
        PanelPack.SetActive(true);
        for (int i = 0; i < Panels.Length; i++)
        {
            if (i == 0) { Panels[i].SetActive(true); }
            else { Panels[i].SetActive(false); }
        }
    }

    public void OnclickNextButton() 
    {
        if (index_ == Panels.Length - 1)
        {
            is_open_ = false;
            PanelPack.SetActive(false);
            PlayerPrefs.SetInt("SawTutorial", 1);
        }
        else {
            index_++;
            Panels[index_].SetActive(true);
        }
    }
}
