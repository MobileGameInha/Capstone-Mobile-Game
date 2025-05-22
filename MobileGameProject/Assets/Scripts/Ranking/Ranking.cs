using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

[System.Serializable]
public class RankerUI
{
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI level;
    public TextMeshProUGUI score;
    public Image characterIcon;
}
public class Ranking : MonoBehaviour
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
    public TextMeshProUGUI MyNickname;
    public TextMeshProUGUI MyTotalScore;
    public TextMeshProUGUI MyStageScore;
    public TextMeshProUGUI MyRanking;
    public TextMeshProUGUI MyLevel;
    [Header("Top Rankers")]
    public List<RankerUI> rankers; 
    public Sprite[] characterSprites;
    // Start is called before the first frame update
    void Start()
    {
         Debug.Log("게임 시작"); 
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
    void UpdateStageText()
    {
        if(StagePage<6&&StagePage>0){
            StageText.text = $"스테이지{StagePage}";    
        }
        else if(StagePage==6){StageText.text = $"흑백모드";}
        else if(StagePage==7){StageText.text = $"무한모드";}
        else if(StagePage==8){StageText.text = $"로테이션모드";}
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
        UpdateStageText();
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
        UpdateStageText();
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
        UpdateStageText();
    }
    public void Toggle_tier(){
        Tier_info.SetActive(!Tier_info.activeSelf);
    }
}
