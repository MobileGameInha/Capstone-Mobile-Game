using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class BasicHelperManager : MonoBehaviour
{
    public static readonly int MAX_HELPER_ = 3;

    public GameObject HelperSelectPanel;
    public GameObject HelperUpgradePanel;

    [SerializeField]
    private int[] selected_cat_index = { -1, -1, -1 };
    [SerializeField]
    private int now_showing_idx = 0;




    private void Start()
    {
        SetCatState();
    }




    public void OnClickOpenSelectPanel(int idx)
    {
        now_showing_idx = idx;
        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(true);
    }

    public void OnClickOpenUpgradePanel()
    {
        HelperUpgradePanel.SetActive(true);
        HelperSelectPanel.SetActive(true);
    }

    public void OnClickCloseSelectPanel()
    {
        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(false);
    }

    public void OnClickCloseUpgradePanel()
    {
        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(true);
    }

    public void OnClickMoveButton(bool isRight)
    {
        for (int i = 0; i < MAX_HELPER_; i++)
        {
            selected_cat_index[i] = DataManager.dataManager.GetSelectedCat(i);
        }

        if (isRight)
        {
            now_showing_idx++;
            if (now_showing_idx >= MAX_HELPER_) { now_showing_idx = 0; }
        }
        else
        {
            now_showing_idx--;
            if (now_showing_idx < 0) { now_showing_idx = 2; }
        }
        ResetHelperSelectPanel();
        ResetHelperUpgradePanel();
    }

    public void OnClickCatButton(int idx)
    {
        if (DataManager.dataManager.GetIsUnlockCat(idx)) 
        {
            selected_cat_index[now_showing_idx] = idx;
            ResetHelperSelectPanel();
            ResetLobbyHelpers();
        }
    }

    public void OnClickCatSelectButton() 
    {
        for (int i = 0; i < MAX_HELPER_; i++)
        {
            if (i != now_showing_idx && selected_cat_index[now_showing_idx] == selected_cat_index[i])
            {
                selected_cat_index[i] = -1;
            }
        }

        DataManager.dataManager.SetSelectedCat(now_showing_idx,selected_cat_index[now_showing_idx]);
        ResetHelperSelectPanel();
    }

    public void OnClickCatUpgradeButton()
    {
        //+)고양이 데이터를 기반으로 업그레이드 진행
    }

    private void ResetLobbyHelpers()
    {
        //+)선택된 고양이 기준으로 화면 상태 변경
    }

    private void ResetHelperSelectPanel() 
    { 
        //+)선택된 고양이 기준으로 화면 상태 변경
    }

    private void ResetHelperUpgradePanel()
    {
        //+)선택된 고양이 기준으로 화면 상태 변경
    }

    private void SetCatState() {
        for (int i = 0; i < MAX_HELPER_; i++)
        {
            selected_cat_index[i] = DataManager.dataManager.GetSelectedCat(i);
        }

        //+)데이터에 따라 고양이 상태 변경

        ResetLobbyHelpers();
    }
}
