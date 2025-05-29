using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
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

    public GameObject WaitingPanel;
    public GameObject ErrorPanel;
    public TMP_Text Error_Text;
    private void Awake()
    {
        ProfileSelectPanel.SetActive(false);
        WaitingPanel.SetActive(false);
        ErrorPanel.SetActive(false);
        OnClickQuickButton(0);
    }

    private void Start()
    {
        ChangeProfileImage();
        ResetState();
        SetAlert();

        if (PlayerPrefs.GetInt("SawTutorial", 0) == 0) {
            GameObject.FindObjectOfType<TutorialManager>().Open();
        }
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
        CoinText.text = DataManager.dataManager.GetCoin().ToString() + "¿ø";
        ExpText.text = DataManager.dataManager.GetRemainEXP().ToString("F2") + "/100";
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
        DataManager.dataManager.SetPlayerProfile(idx);
        ChangeProfileImage();
        ProfileSelectPanel.SetActive(false);
    }

    private void ChangeProfileImage() 
    {
        profileImage.sprite = profileSprites[DataManager.dataManager.GetProfileImage()];
    }

    public void OpenError(string error)
    {
        Error_Text.text = error;
        ErrorPanel.SetActive(true);
    }

    public void CloseError()
    {
        ErrorPanel.SetActive(false);
    }

    public void OpenWaiting()
    {
        WaitingPanel.SetActive(true);
    }

    public void CloseWaiting()
    {
        WaitingPanel.SetActive(false);
    }
}
