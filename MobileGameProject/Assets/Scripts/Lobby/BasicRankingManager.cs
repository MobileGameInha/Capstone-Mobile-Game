using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

[System.Serializable]
public class RankerUI
{
    public TMP_Text nickname;
    public TMP_Text level;
    public TMP_Text score;
    public UnityEngine.UI.Image profileImage;
}
public class BasicRankingManager : MonoBehaviour
{
    [Header("Stage_page")]
    public List<GameObject>Pages;
    public GameObject Leftbtn;
    public GameObject Rightbtn;
    private int CurrentPage=0;
    private int StagePage=1;
    public TextMeshProUGUI StageText;
    public GameObject StageUI;
    public GameObject Stage_rank;
    public GameObject Tier_info;
    [Header("Myinfo")]
    public TMP_Text MyNickname;
    public TMP_Text MyTotalScore;
    public UnityEngine.UI.Image MyTierImage;
    public UnityEngine.UI.Image MyProfileImage;
    [Header("MyinfoInRank")]
    public TMP_Text MyNicknameInRank;
    public TMP_Text MyScoreInRank;
    public TMP_Text MyRankInRank;
    public UnityEngine.UI.Image MyProfileImageInRank;
    [Header("Top Rankers")]
    public List<RankerUI> rankers; 
    // Start is called before the first frame update

    public Sprite[] tierSprites;
    public Sprite[] profileSprites;
    public Sprite[] profileFullSprites;

    public GameObject WatingPanel;
    public GameObject LockPanel;


    private void Awake()
    {
        DataManager.dataManager.requestSuccededDelegateForRank += SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegateForRank += FailRequestEvent;
    }
    private void OnDestroy()
    {
        if (DataManager.dataManager != null)
        {
            DataManager.dataManager.requestSuccededDelegateForRank -= SuccessRequestEvent;
            DataManager.dataManager.requestFailedDelegateForRank -= FailRequestEvent;
        }
    }

    private void Start()
    {
        WatingPanel.SetActive(true);
        LockPanel.SetActive(false);

        DataManager.dataManager.GetRankingData();
    }




    // Update is called once per frame
    public void ShowTab(int index){
        if(index>0){
            StagePage=index;
            Onclick_Stage(index);
        }
    }
    public void ShowPage(int index)
    {
        ShowArrow(index);
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
    void UpdateStage()
    {
        if(StagePage<6&&StagePage>0){
            StageText.text = $"스테이지{StagePage}";    
        }
        else if(StagePage==6){StageText.text = $"흑백모드";}
        else if(StagePage==7){StageText.text = $"무한모드";}
        else if(StagePage==8){StageText.text = $"로테이션모드";}


        MyScoreInRank.text = DataManager.dataManager.GetPlayerMaxScore(StagePage - 1).ToString();
        MyRankInRank.text = DataManager.dataManager.GetPlayerRank(StagePage - 1).ToString() + "등";

        for (int i = 0; i < 3; i++)
        {
            rankers[i].nickname.text = DataManager.dataManager.GetNicknameOfRanker(StagePage - 1, i);
            rankers[i].level.text = DataManager.dataManager.GetLevelOfRanker(StagePage - 1, i).ToString();
            rankers[i].score.text = DataManager.dataManager.GetScoreOfRanker(StagePage - 1, i).ToString();
            rankers[i].profileImage.sprite = profileFullSprites[DataManager.dataManager.GetProfileOfRanker(StagePage - 1, i)];
        }
    }
    public void RestoreStagePage(int index)
    {
        ShowArrow(index);
        for (int i = 0; i < Pages.Count; i++)
            Pages[i].SetActive(i == index);
    }
    public void OnClickNext_stage()
    {
        if (StagePage==8)
        {
            StagePage=1;
        }
        else
        {
            StagePage++;
        }
        UpdateStage();
        //RestoreStagePage(StagePage);
    }   

    public void OnClickPrev_stage()
    {
        if (StagePage==1)
        {
            StagePage=8;
        }
        else
        {
            StagePage--;
        }
        UpdateStage();
        //RestoreStagePage(StagePage);
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
    public void Onclick_Stage(int stageNumber){
        Stage_rank.SetActive(!Stage_rank.activeSelf);
        StageUI.SetActive(!StageUI.activeSelf);
        StagePage=stageNumber;
        UpdateStage();
    }
    public void Toggle_tier(){
        Tier_info.SetActive(!Tier_info.activeSelf);
    }



    private void SuccessRequestEvent()
    {
        WatingPanel.SetActive(false);
        LockPanel.SetActive(false);

        MyNickname.text = DataManager.dataManager.GetNickName();
        MyTotalScore.text = DataManager.dataManager.GetPlayerTotalScore().ToString();
        MyTierImage.sprite = tierSprites[DataManager.dataManager.GetPlayerTier()];
        MyProfileImage.sprite = profileSprites[DataManager.dataManager.GetProfileImage()];

        MyNicknameInRank.text = DataManager.dataManager.GetNickName();
        MyProfileImageInRank.sprite = profileSprites[DataManager.dataManager.GetProfileImage()];
    }

    private void FailRequestEvent(string err)
    {
        WatingPanel.SetActive(false);
        LockPanel.SetActive(true);
    }


}
