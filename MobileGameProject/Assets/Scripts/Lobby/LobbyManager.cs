using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Image profileImage;

    public Sprite[] profileSprites;

    public TMP_Text NickNameText;
    public TMP_Text LevelText;
    public TMP_Text CoinText;
    public TMP_Text ExpText;

    public Slider EXPSlider;

    public GameObject[] MenuPacks;
    public GameObject[] QuickButtonBorders;

    public GameObject ProfileSelectPanel;

    public GameObject[] StageAlert;
    public GameObject ChallangeAlert;

    private void Awake()
    {
        ProfileSelectPanel.SetActive(false);
        OnClickQuickButton(0);
    }

    private void Start()
    {
        ChangeProfileImage();
        ResetState();
        SetAlert();
    }

    private void SetAlert() {
        for (int i = 0; i < 4; i++)
        {
            if (PlayerPrefs.GetInt("NoticeAlert_" + (i + 2).ToString(), 0) == 0 && DataManager.dataManager.GetIsUnlockStage(i + 1))
            {
                PlayerPrefs.SetInt("NoticeAlert_" + (i + 2).ToString(), 1);
                StageAlert[i].SetActive(true);
            }
            else {
                StageAlert[i].SetActive(false);
            }
        }

        if (PlayerPrefs.GetInt("NoticeAlert_Challange", 0) == 0 && DataManager.dataManager.GetIsUnlockChallangeStage())
        {
            PlayerPrefs.SetInt("NoticeAlert_Challange", 1);
            ChallangeAlert.SetActive(true);
        }
        else {
            ChallangeAlert.SetActive(false);
        }
    }

    public void ResetState() {
        NickNameText.text = DataManager.dataManager.GetNickName();
        LevelText.text = "LV " + DataManager.dataManager.GetLevel().ToString();
        CoinText.text = DataManager.dataManager.GetCoin().ToString() + "원";
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

    public void OnClickProfileSelectButton()
    {
        ProfileSelectPanel.SetActive(!ProfileSelectPanel.activeSelf);
    }

    public void OnClickProfileChangeButton(int idx)
    {
        //+)프로필 값 변경
        ChangeProfileImage();
        ProfileSelectPanel.SetActive(false);
    }

    private void ChangeProfileImage() 
    {
        profileImage.sprite = profileSprites[DataManager.dataManager.GetProfileImage()];
    }
}
