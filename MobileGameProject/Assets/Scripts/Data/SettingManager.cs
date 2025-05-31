using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public TutorialManager tutorialManager;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Volume Sliders")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("Setting UI Button")]
    public GameObject settingUI;
    public Button LowFPS;
    public Button MediumFPS;
    public Button HighFPS;

    [Header("Color of Buttons")]
    public Color UnSelectedText;
    public Color UnSelectedFrame;
    public Color SelectedText;
    public Color SelectedFrame=Color.white;

    [Header("CutScene")]
    public List<GameObject>Pages;
    public GameObject Leftbtn;
    public GameObject Rightbtn;
    private int CurrentPage=0;
    public TextMeshProUGUI PageText;
    public GameObject CutScneUI;
    
    [Header("Panel Prompt")]
    public GameObject promptPanel; 
    public GameObject blurBackground;
    public TextMeshProUGUI promptTitleText;
    public enum PromptType { Logout, Quit }
    private PromptType currentPromptType;

    private void Start()
    {

        //기존에 유저가 설정한 설정값대로 초기 설정값 설정
        // === 사운드 ===
        float master = PlayerPrefs.GetFloat("MasterVolume");
        float bgm = PlayerPrefs.GetFloat("BGMVolume");
        float sfx = PlayerPrefs.GetFloat("SFXVolume"); 

        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;

        SetMasterVolume(master);
        SetBgmVolume(bgm);
        SetSfxVolume(sfx);

        // === 프레임 설정 ===
        int savedFPS = PlayerPrefs.GetInt("FPSSetting");
        switch(savedFPS){
            case 30:
                SetLowFps();
                break;
            case 60:
                SetMediumFps();
                break;
            default:
                SetHighFps();
                break;
        }

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        

        
    }
    

    void ChangeButtonColor(Button myButton,Color newColor,Color newTextColor)
    {
        Image btnImage = myButton.GetComponent<Image>();
        if (btnImage != null)
        {
            btnImage.color = newColor;
            
        }
        TextMeshProUGUI btnTMPText = myButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnTMPText != null)
        {
            btnTMPText.color = newTextColor;
        }
    }
    public void SetMasterVolume(float value)
    {
        
        float dB = Mathf.Lerp(-80f, 0f, value);
        audioMixer.SetFloat("Master", dB);
        PlayerPrefs.SetFloat("MasterVolume", value);
        Debug.Log("마스터 볼륨 설정: "+value);
    }

    public void SetBgmVolume(float value)
    {
        float dB = Mathf.Lerp(-80f, 0f, value);
        audioMixer.SetFloat("BGM", dB);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void SetSfxVolume(float value)
    {
        float dB = Mathf.Lerp(-80f, 0f, value);
        audioMixer.SetFloat("SFX", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetLowFps()
    {
        Application.targetFrameRate = 30;
        Debug.Log("프레임 설정: Low (30 FPS)");
        PlayerPrefs.SetInt("FPSSetting", 30);
        ChangeButtonColor(LowFPS,SelectedFrame,SelectedText);
        ChangeButtonColor(MediumFPS,UnSelectedFrame,UnSelectedText);
        ChangeButtonColor(HighFPS,UnSelectedFrame,UnSelectedText);
    }

    public void SetMediumFps()
    {
        Application.targetFrameRate = 60;
        Debug.Log("프레임 설정: Medium (60 FPS)");
        PlayerPrefs.SetInt("FPSSetting", 60);
        ChangeButtonColor(LowFPS,UnSelectedFrame,UnSelectedText);
        ChangeButtonColor(MediumFPS,SelectedFrame,SelectedText);
        ChangeButtonColor(HighFPS,UnSelectedFrame,UnSelectedText);
    }

    public void SetHighFps()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        Debug.Log("프레임 설정: High (-1 FPS)");
        PlayerPrefs.SetInt("FPSSetting", -1);
        ChangeButtonColor(LowFPS,UnSelectedFrame,UnSelectedText);
        ChangeButtonColor(MediumFPS,UnSelectedFrame,UnSelectedText);
        ChangeButtonColor(HighFPS,SelectedFrame,SelectedText);
    }

    public void ToggleSettingUI()
    {
        if (settingUI.activeSelf == false) { Time.timeScale = 0f; } else { Time.timeScale = 1f; }
        settingUI.SetActive(!settingUI.activeSelf);
    }
    public void ToggleCutSceneUI()
    {
        CutScneUI.SetActive(!CutScneUI.activeSelf);
    }

    public void ShowPage(int index)
    {
        ShowArrow(index);
        UpdatePageText();
        for (int i = 0; i < Pages.Count; i++)
            Pages[i].SetActive(i == index);
    }

    public void OnClickNext()
    {
        if (CurrentPage < Pages.Count - 1)
        {
            CurrentPage++;
            ShowPage(CurrentPage);
        }
    }   

    public void OnClickPrev()
    {
        if (CurrentPage > 0)
        {
            CurrentPage--;
            ShowPage(CurrentPage);
        }
    }
    void UpdatePageText()
    {
        PageText.text = $"{CurrentPage + 1}";
    }

    public void OnClickTutorial() {
        tutorialManager.Open();
        ToggleSettingUI();
    }

    public void ShowArrow(int index)
    {
        if (index==0){
            Leftbtn.SetActive(false);
            Rightbtn.SetActive(true);
        }
        else{
            Leftbtn.SetActive(true);
            Rightbtn.SetActive(false);
        }
    }
    public void OnLogoutButtonClicked()
    {
        currentPromptType = PromptType.Logout;
        ShowPrompt("로그아웃 하시겠습니까?");
    }

    public void OnQuitGameButtonClicked()
    {
        currentPromptType = PromptType.Quit;
        ShowPrompt("게임을 종료하시겠습니까?");
    }

    private void ShowPrompt(string message)
    {
        promptTitleText.text = message;
        promptPanel.SetActive(true);
        blurBackground.SetActive(true);
    }

    public void OnConfirmPrompt()
    {
        switch (currentPromptType)
        {
            case PromptType.Logout:
                DataManager dm = FindObjectOfType<DataManager>();
                GameObject.Destroy(dm.gameObject);
                DontDestroyOnLoad[] ddls = FindObjectsOfType<DontDestroyOnLoad>();
                for (int i = 0; i < ddls.Length; i++)
                {
                    GameObject.Destroy(ddls[i].gameObject);
                }

                PlayerPrefs.DeleteKey("saved_id");
                PlayerPrefs.DeleteKey("saved_pw");
                PlayerPrefs.Save();
                Debug.Log("로그아웃 완료");
                LoadingManager.LoadScene("TitleScene");
                break;

            case PromptType.Quit:
                Debug.Log("게임 종료");
                Application.Quit();
                break;
        }
        Time.timeScale = 1f;
        settingUI.SetActive(false);
        ClosePrompt();
    }

    public void ClosePrompt()
    {
        promptPanel.SetActive(false);
        blurBackground.SetActive(false);
    }
}
