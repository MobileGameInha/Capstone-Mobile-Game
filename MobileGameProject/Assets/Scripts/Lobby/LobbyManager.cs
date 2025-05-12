using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text NickNameText;
    public TMP_Text LevelText;
    public TMP_Text CoinText;
    public TMP_Text ExpText;

    public Slider EXPSlider;

    public GameObject[] MenuPacks;
    public GameObject[] QuickButtonBorders;

    private void Awake()
    {
        OnClickQuickButton(0);
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState() {
        NickNameText.text = DataManager.dataManager.GetNickName();
        LevelText.text = "LV " + DataManager.dataManager.GetLevel().ToString();
        CoinText.text = DataManager.dataManager.GetCoin().ToString() + "¿ø";
        ExpText.text = DataManager.dataManager.GetRemainEXP().ToString("F2") + "/500";
        EXPSlider.value = DataManager.dataManager.GetRemainEXP() / DataManager.PLAYER_PER_EXP;
    }

    public void OnClickQuickButton(int idx) {
        for (int i = 0; i < MenuPacks.Length; i++)
        {
            if (i == idx) 
            { 
                MenuPacks[i].SetActive(true);
                QuickButtonBorders[i].SetActive(true);
            }
            else { 
                MenuPacks[i].SetActive(false);
                QuickButtonBorders[i].SetActive(false);
            } 
        }
    }

}
