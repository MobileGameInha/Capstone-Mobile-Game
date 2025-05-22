using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class BasicHelperManager : MonoBehaviour
{
    public static readonly string[] CAT_NAME_LIST_ = 
        { "�丮", "�Ʒ�", "�糪", "�ֶ�", "���", "��Ű",
    "����", "����", "����", "�þ�", "����", "�ູ��"};

    public static readonly string[] CAT_EXPLAIN_LIST_ =
        { "������ �� ���ѽð���\n�÷��ݴϴ�.", "������ ���� �ð���\n�÷��ݴϴ�.", "Ÿ���� �þ�� �ӵ���\n�����մϴ�.", "�Ǽ��ص� CO2��������\n�� �����մϴ�.",
        "����ġ ȹ�淮��\n�����մϴ�.", "��� ȹ�淮��\n�����մϴ�.", "�� ���� �Ǽ���\n�����˴ϴ�.", "�ǹ� Ƚ����\n�����մϴ�.", "���� Ȯ����\n���ʽ� ���������� �����մϴ�.",
        "���� Ȯ����\n�� ���� �ܼ�ȭ �մϴ�.", "���� Ȯ����\n���� �ð��� ���� �մϴ�.", "���� Ȯ����\n�����ڸ� �����մϴ�."};

    public static readonly int[,] CAT_UPGRADE_LIST_ =
    {
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},

        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},

        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET},
        { (int)Item.BELL,(int)Item.BOX,(int)Item.TICKET}
    };

    public static readonly int[] CAT_UPGRATE_COUNT_ = { 3, 4, 5, 6, 7, 0 };

    public static readonly int MAX_HELPER_ = 3;

    public GameObject HelperSelectPanel;
    public GameObject HelperUpgradePanel;
    public GameObject HelperButtons;


    [SerializeField]
    private int[] selected_cat_index = { -1, -1, -1 };
    [SerializeField]
    private int now_showing_idx = 0;

    public SkeletonAnimation[] IdleCats = new SkeletonAnimation[3];
    public GameObject[] NoneImages = new GameObject[3];

    public SkeletonAnimation HelperSelect_SelectedCat;
    public SkeletonAnimation[] HelperSelect_IdleCats = new SkeletonAnimation[GameManager.CAT_SIZE_];
    public TMP_Text CatNameText;
    public TMP_Text CatExplainText;
    public Slider CatEXPSlider;
    public GameObject CatEXPIcon;
    public TMP_Text CatLevelText;

    public SkeletonAnimation Upgrade_SelectedCat;
    public TMP_Text Upgrade_CatNameText;
    public Slider Upgrade_CatEXPSlider;
    public GameObject Upgrade_CatEXPIcon;
    public TMP_Text Upgrade_CatLevelText;
    public Image[] Upgrade_ItemImages;
    public TMP_Text[] Upgrade_ItemCountText;
    public GameObject Upgrade_UpgradeButton;
    public GameObject Upgrade_UpgradeCheckPanel;
    public GameObject Upgrade_UpgradeLevelCheckPanel;

    public Sprite[] Upgrade_ItemSprites;

    private void Start()
    {
        SetCatState();
    }




    public void OnClickOpenSelectPanel(int idx)
    {
        now_showing_idx = idx;

        ResetHelperSelectPanel();

        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(true);
        HelperButtons.SetActive(false);
    }

    public void OnClickOpenUpgradePanel()
    {
        if (selected_cat_index[now_showing_idx] == -1) { return; }

        ResetHelperUpgradePanel();

        HelperUpgradePanel.SetActive(true);
        HelperSelectPanel.SetActive(false);
        HelperButtons.SetActive(false);
    }

    public void OnClickCloseSelectPanel()
    {
        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(false);
        HelperButtons.SetActive(true);
    }

    public void OnClickCloseUpgradePanel()
    {
        ResetHelperSelectPanel();

        HelperUpgradePanel.SetActive(false);
        HelperSelectPanel.SetActive(true);
        HelperButtons.SetActive(false);
    }

    public void OnClickMoveButton(bool isRight)
    {
        for (int i = 0; i < MAX_HELPER_; i++)
        {
            selected_cat_index[i] = DataManager.dataManager.GetSelectedCat(i);
        }

        int before = selected_cat_index[now_showing_idx];

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
        ResetHelperSelectPanel(true, before);
        ResetHelperUpgradePanel();
    }

    public void OnClickCatButton(int idx)
    {
        if (DataManager.dataManager.GetIsUnlockCat(idx)) 
        {
            int before = selected_cat_index[now_showing_idx];
            selected_cat_index[now_showing_idx] = idx;
            ResetHelperSelectPanel(true, before);
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
        ResetLobbyHelpers();
        GameObject.FindObjectOfType<BasicStageManagement>().ResetCatState();
        GameObject.FindObjectOfType<BasicChallangeManager>().ResetCatState();
    }

    public void OnClickCatUpgradeButton()
    {
        //+)����� �����͸� ������� ���׷��̵� ����
    }

    private void ResetLobbyHelpers()
    {
        for (int i = 0; i < MAX_HELPER_; i++)
        {
            if (selected_cat_index[i] != -1)
            {
                NoneImages[i].SetActive(false);
                IdleCats[i].gameObject.SetActive(true);
                IdleCats[i].initialSkinName = "Cat-" + (selected_cat_index[i] + 1).ToString();
                IdleCats[i].Initialize(true);
                if (IdleCats[i].AnimationState != null)
                    IdleCats[i].AnimationState.SetAnimation(0, ("idle-" + (Random.Range(1, 26)).ToString()), true);
            }
            else
            {
                NoneImages[i].SetActive(true);
                IdleCats[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResetHelperSelectPanel(bool already_in_panel = false, int before_index = -1) 
    {
        if (!already_in_panel)
        {
            for (int i = 0; i < GameManager.CAT_SIZE_; i++)
            {
                if (DataManager.dataManager.GetIsUnlockCat(i))
                {
                    HelperSelect_IdleCats[i].Initialize(true);
                    HelperSelect_IdleCats[i].AnimationState.SetAnimation(0, "idle-1", true);
                }
                else
                {
                    HelperSelect_IdleCats[i].Initialize(true);
                    HelperSelect_IdleCats[i].AnimationState.SetAnimation(0, "idle-2", true);
                }
            }
        }
        else if(before_index!=-1) 
        {

                if (DataManager.dataManager.GetIsUnlockCat(before_index))
                {
                    HelperSelect_IdleCats[before_index].Initialize(true);
                    HelperSelect_IdleCats[before_index].AnimationState.SetAnimation(0, "idle-1", true);
                }
                else
                {
                    HelperSelect_IdleCats[before_index].Initialize(true);
                    HelperSelect_IdleCats[before_index].AnimationState.SetAnimation(0, "idle-2", true);
                }

        }

        

        if (selected_cat_index[now_showing_idx] != -1)
        {
            HelperSelect_IdleCats[selected_cat_index[now_showing_idx]].AnimationState.SetAnimation(0, "idle-11", true);

            HelperSelect_SelectedCat.gameObject.SetActive(true);
            HelperSelect_SelectedCat.initialSkinName = "Cat-" + (selected_cat_index[now_showing_idx] + 1).ToString();
            HelperSelect_SelectedCat.Initialize(true);

            CatEXPSlider.value = DataManager.dataManager.GetEXPOfCat(selected_cat_index[now_showing_idx]) / DataManager.MAX_CAT_EXP;

            CatLevelText.text = DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx]).ToString() + " / 5";


            if (DataManager.dataManager.GetEXPOfCat(selected_cat_index[now_showing_idx]) == 100.0f)
            {
                CatEXPIcon.SetActive(true);
            }
            else
            {
                CatEXPIcon.SetActive(false);
            }

            CatExplainText.text = CAT_EXPLAIN_LIST_[selected_cat_index[now_showing_idx]];
            CatNameText.text = CAT_NAME_LIST_[selected_cat_index[now_showing_idx]];
        }
        else
        {
            HelperSelect_SelectedCat.gameObject.SetActive(false);

            CatEXPIcon.SetActive(false);
            CatEXPSlider.value = 0.0f;

            CatLevelText.text = "";

            CatExplainText.text = "";
            CatNameText.text = "";
        }
    }//���� ����Ʈ ��������


    public void ResetHelperUpgradePanel()
    {
        if (selected_cat_index[now_showing_idx] != -1)
        {

            Upgrade_SelectedCat.gameObject.SetActive(true);
            Upgrade_SelectedCat.gameObject.SetActive(true);
            Upgrade_SelectedCat.initialSkinName = "Cat-" + (selected_cat_index[now_showing_idx] + 1).ToString();
            Upgrade_SelectedCat.Initialize(true);

            Upgrade_CatEXPSlider.value = DataManager.dataManager.GetEXPOfCat(selected_cat_index[now_showing_idx]) / DataManager.MAX_CAT_EXP;

            Upgrade_CatLevelText.text = DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx]).ToString() + " / 5";

            if (DataManager.dataManager.GetEXPOfCat(selected_cat_index[now_showing_idx]) == 100.0f)
            {
                Upgrade_CatEXPIcon.SetActive(true);
            }
            else
            {
                Upgrade_CatEXPIcon.SetActive(false);
            }

            for (int i = 0; i < Upgrade_ItemImages.Length; i++)
            {
                Upgrade_ItemImages[i].gameObject.SetActive(true);
                Upgrade_ItemImages[i].sprite = Upgrade_ItemSprites[CAT_UPGRADE_LIST_[selected_cat_index[now_showing_idx], i]];
                Upgrade_ItemCountText[i].text = DataManager.dataManager.GetItemCount((Item)CAT_UPGRADE_LIST_[selected_cat_index[now_showing_idx], i]).ToString() + "/" + CAT_UPGRATE_COUNT_[DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx])].ToString();
            }


            if (DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx]) == 5)
            {
                Upgrade_UpgradeLevelCheckPanel.SetActive(true);
                Upgrade_UpgradeButton.SetActive(false);
                Upgrade_UpgradeCheckPanel.SetActive(false);
            }
            else if (DataManager.dataManager.GetEXPOfCat(selected_cat_index[now_showing_idx]) == 100.0f &&
                DataManager.dataManager.GetItemCount((Item)CAT_UPGRADE_LIST_[selected_cat_index[now_showing_idx], 0]) >= CAT_UPGRATE_COUNT_[DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx])] &&
                DataManager.dataManager.GetItemCount((Item)CAT_UPGRADE_LIST_[selected_cat_index[now_showing_idx], 1]) >= CAT_UPGRATE_COUNT_[DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx])] &&
                DataManager.dataManager.GetItemCount((Item)CAT_UPGRADE_LIST_[selected_cat_index[now_showing_idx], 2]) >= CAT_UPGRATE_COUNT_[DataManager.dataManager.GetLevelOfCat(selected_cat_index[now_showing_idx])]
                )
            {
                Upgrade_UpgradeLevelCheckPanel.SetActive(false);
                Upgrade_UpgradeButton.SetActive(true);
                Upgrade_UpgradeCheckPanel.SetActive(false);
            }
            else
            {
                Upgrade_UpgradeLevelCheckPanel.SetActive(false);
                Upgrade_UpgradeButton.SetActive(false);
                Upgrade_UpgradeCheckPanel.SetActive(true);
            }

            Upgrade_CatNameText.text = CAT_NAME_LIST_[selected_cat_index[now_showing_idx]];
        }
        else 
        {
            Upgrade_SelectedCat.gameObject.SetActive(false);

            for (int i = 0; i < Upgrade_ItemImages.Length; i++)
            {
                Upgrade_ItemImages[i].gameObject.SetActive(false);
                Upgrade_ItemCountText[i].text = "";
            }

            Upgrade_UpgradeLevelCheckPanel.SetActive(false);
            Upgrade_UpgradeButton.SetActive(false);
            Upgrade_UpgradeCheckPanel.SetActive(false);

            Upgrade_CatEXPSlider.value = 0.0f;
            Upgrade_CatLevelText.text = "";
            Upgrade_CatEXPIcon.SetActive(false);
            Upgrade_CatNameText.text = "";
        }
    }

    private void SetCatState() {

        for (int i = 0; i < MAX_HELPER_; i++)
        {
            selected_cat_index[i] = DataManager.dataManager.GetSelectedCat(i);
        }

        ResetLobbyHelpers();
    }
}
