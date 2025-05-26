using MinigamesDemo;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicShopManager : MonoBehaviour
{
    private readonly int SHOW_PARAM_HASH = Animator.StringToHash("SHOW");

    public static readonly int[] CAT_COST_LIST =
    { 100,200,200,300,300,
    400,400,500,500,600,
    600,600};

    public static readonly int ITEM_COUNT = 8;

    public LobbyManager lobbyManager;

    public GameObject MainPanel;
    public GameObject PetPanel;
    public GameObject ItemPanel;

    public TMP_Text PetName;
    public TMP_Text PetExplain;
    public TMP_Text PetCost;
    public SkeletonAnimation Cat;

    public GameObject BuyPromptPanel;

    public Animator BuyAnimator;
    public Animator AlreadyBuyAnimator;
    public Animator UnderCostAnimator;
    public Animator UnderCostWheelAnimator;

    public GameObject InventoryPanel;

    public TMP_Text[] ItemCountTexts;

    public FortuneWheel Wheel;

    private int pet_index_ = 0;

    private bool isSpeening = false;

    private enum RequestType { BuyCat, GetItem};
    private RequestType request_type_;
    private bool is_requesting_ = false;
    private void Awake()
    {
        MainPanel.SetActive(true);
        PetPanel.SetActive(false);
        ItemPanel.SetActive(false);
        BuyPromptPanel.SetActive(false);

        DataManager.dataManager.requestSuccededDelegate += SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate += FailRequestEvent;

        pet_index_ = 0;

        is_requesting_ = false;
    }

    private void OnDestroy()
    {
        DataManager.dataManager.requestSuccededDelegate -= SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate -= FailRequestEvent;
    }

    private void Start()
    {
        ResetPetState();
        ResetItemState();
    }

    public void OnClickPetshopButton() 
    {
        MainPanel.SetActive(false);
        PetPanel.SetActive(true);
        ItemPanel.SetActive(false);
    }

    public void OnClickItemshopButton()
    {
        MainPanel.SetActive(false);
        PetPanel.SetActive(false);
        ItemPanel.SetActive(true);
    }

    public void OnClickReturnToSelectButton()
    {
        MainPanel.SetActive(true);
        PetPanel.SetActive(false);
        ItemPanel.SetActive(false);
    }

    public void OnClickPetShopRightButton()
    {
        pet_index_++;
        if (pet_index_ >= GameManager.CAT_SIZE_) { pet_index_ = 0; }
        ResetPetState();
    }

    public void OnClickPetShopLeftButton()
    {
        pet_index_--;
        if (pet_index_ < 0) { pet_index_ = GameManager.CAT_SIZE_ - 1; }
        ResetPetState();
    }

    public void OnClickPetShopBuyButton()
    {
        if (DataManager.dataManager.GetIsUnlockCat(pet_index_)) 
        {
            AlreadyBuyAnimator.SetTrigger(SHOW_PARAM_HASH);
        }
        else if (DataManager.dataManager.GetCoin() < CAT_COST_LIST[pet_index_])
        {
            UnderCostAnimator.SetTrigger(SHOW_PARAM_HASH);
        }
        else
        {
            BuyPromptPanel.SetActive(true);
        }
    }

    public void OnClickPetShopBuyButtonYes()
    {
        if (!DataManager.dataManager.GetIsRequesting())
            SendCatBuyRequest();
    }

    public void OnClickPetShopBuyButtonNo()
    {
        BuyPromptPanel.SetActive(false);
    }

    public void ResetPetState() {
        if (pet_index_ >= 0 && pet_index_ < GameManager.CAT_SIZE_)
        {
            PetName.text = BasicHelperManager.CAT_NAME_LIST_[pet_index_];
            PetExplain.text = BasicHelperManager.CAT_EXPLAIN_LIST_[pet_index_];
            if (DataManager.dataManager.GetIsUnlockCat(pet_index_))
            {
                PetCost.text = "함께하는\n고양이";
            }
            else
            {
                PetCost.text = CAT_COST_LIST[pet_index_].ToString() + "원";
            }
            Cat.initialSkinName = "Cat-" + (pet_index_ + 1).ToString();
            Cat.Initialize(true);
        }
    }


    public void ResetItemState()
    {
        for (int i = 0; i < ITEM_COUNT; i++)
        {
            ItemCountTexts[i].text = DataManager.dataManager.GetItemCount((Item)i).ToString();
        }
    }
    public void OnClickWheelButton() {
        if (isSpeening) return;

        if (DataManager.dataManager.GetIsRequesting()) return;

        if (DataManager.dataManager.GetCoin() < DataManager.ITEM_PRICE)
        {
            UnderCostWheelAnimator.SetTrigger(SHOW_PARAM_HASH);
        }
        else {
            isSpeening = true;

            Wheel.Spin();

            ResetItemState();
            GameObject.FindObjectOfType<BasicHelperManager>().ResetHelperUpgradePanel();
        }
    }

    public void OnClickInventoryButton()
    {
        InventoryPanel.SetActive(true);
    }

    public void OnClickInventoryExitButton()
    {
        InventoryPanel.SetActive(false);
    }

    public void AddItem(int idx) 
    {
        Debug.Log(idx.ToString() + "아이템 획득");
        is_requesting_ = true;
        request_type_ = RequestType.GetItem;
        lobbyManager.OpenWaiting();
        DataManager.dataManager.GetItem(idx);
    }





    private void SendCatBuyRequest() {
        is_requesting_ = true;
        request_type_ = RequestType.BuyCat;
        lobbyManager.OpenWaiting();
        DataManager.dataManager.UnlockCat(pet_index_);
    }


    private void SuccessRequestEvent()
    {
        if (!is_requesting_) return;

        is_requesting_ = false;

        lobbyManager.CloseWaiting();

        Debug.Log("서버요청 성공 이벤트 발생");
        switch (request_type_)
        {
            case RequestType.BuyCat:
                BuyAnimator.SetTrigger(SHOW_PARAM_HASH);
                BuyPromptPanel.SetActive(false);
                ResetPetState();
                lobbyManager.ResetState();
                GameObject.FindObjectOfType<BasicHelperManager>().ResetHelperSelectPanel();
                break;
            case RequestType.GetItem:
                ResetItemState();
                lobbyManager.ResetState();
                GameObject.FindObjectOfType<BasicHelperManager>().ResetHelperUpgradePanel();
                break;
            default:
                break;
        }

    }

    private void FailRequestEvent(string err)
    {
        if (!is_requesting_) return;

        is_requesting_ = false;

        lobbyManager.CloseWaiting();
        lobbyManager.OpenError(err);
    }

}
