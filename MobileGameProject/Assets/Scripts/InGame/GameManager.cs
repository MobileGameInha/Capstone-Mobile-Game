using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;


public enum ChallangeMode { None, ButtonRoatator, OneLife, BlackAndWhite}
public enum ArrowDirection { LU, LD, RU, RD, CTORUP, DN }

public static class DisruptorIndex
{
    public const int BUTTON_SWAP_ = 0;
    public const int HIDE_ = 1;
    public const int TIME_REMOVE_ = 2;
    public const int HARD_FEVER_ = 3;
}


public static class CatIndex {
    public const int TOTAL_TIME_UP_ = 0;
    public const int ROUND_TIME_UP_ = 1;
    public const int TILE_SPEED_DOWN_ = 2;
    public const int LIFE_REMOVE_DOWN_ = 3;
    public const int EXP_UP_ = 4;

    public const int GOLD_UP_ = 5;
    public const int MISTAKE_DEFENCE_ = 6;
    public const int FEVER_UP_ = 7;
    public const int BONUS_STAGE_ = 8;
    public const int SIMPLE_LINE_ = 9;

    public const int TIME_STOP_ = 10;
    public const int SAVOTAGE_DEFENCE_ = 11;
}

public static class CatValue
{
    public static readonly float[] TOTAL_TIME_UP_ = new float[] {1,2,3,4,5 };
    public static readonly float[] ROUND_TIME_UP_ = new float[] { 1, 2, 3, 4, 5 };
    public static readonly float[] TILE_SPEED_DOWN_ = new float[] { 1, 2, 3, 4, 5 };
    public static readonly float[] LIFE_REMOVE_DOWN_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };
    public static readonly float[] EXP_UP_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };

    public static readonly float[] GOLD_UP_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };
    public static readonly float[] MISTAKE_DEFENCE_ = new float[] { 1, 2, 3, 4, 5 };
    public static readonly float[] FEVER_UP_ = new float[] { 1, 2, 3, 4, 5 };
    public static readonly float[] BONUS_STAGE_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };
    public static readonly float[] SIMPLE_LINE_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };

    public static readonly float[] TIME_STOP_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };
    public static readonly float[] SAVOTAGE_DEFENCE_ = new float[] { 0.2f, 0.25f, 0.3f, 0.4f, 0.5f };
}


public class GameManager : MonoBehaviour
{
    public static GameManager gameManager
    {
        get
        {
            if (gameManager_instance == null)
            {
                gameManager_instance = FindObjectOfType<GameManager>();
            }

            return gameManager_instance;
        }
    }//GameManager�� �̱������� ����

    private static GameManager gameManager_instance; //�̱��� �ν��Ͻ�


    public TMP_Text ScoreText; //���ھ� �ؽ�Ʈ : �ܺ� ����
    public Slider TotalTimerSlider;
    public Slider RoundTimerSlider;

    [SerializeField]
    private int stageID = 0;

    [SerializeField]
    private ChallangeMode Mode = ChallangeMode.None;

    [SerializeField]
    private ButtonRotator CHALLANGE_ButtonRotator;

    [SerializeField]
    private TileManager tile_manager_;
    [SerializeField]
    private LayerManager layer_manager_;
    [SerializeField]
    private VisualManager visual_manager_;
    [SerializeField]
    private InGameBGMManager bgm_manager_;



    private readonly int SWAP_BUTTONS_HASH = Animator.StringToHash("SWAP");
    [SerializeField]
    private Animator ButtonSwapAnimator;
    private bool is_swap_ = false;
    //[SerializeField]
    //private Transform LeftUpButtonTransform;
    //[SerializeField]
    //private Transform RightUpButtonTransform;
    //[SerializeField]
    //private Transform LeftDownButtonTransform;
    //[SerializeField]
    //private Transform RightDownButtonTransform;

    private bool is_started_ = false; //���� ���� ����

    private bool is_perfect_ = true; //����Ʈ ����

    private int score_ = 0; //����
    private const int ADDING_SCORE_TILE_= 10; //Ÿ�� ����
    private const int ADDING_SCORE_PERFECT_ = 20; //����Ʈ ����
    private const int REMOVING_SCORE_TILE_ = -5; //����


    private float co2_ = 50; //CO2
    private const float MAX_CO2_ = 100; //CO2 �ִ밪
    private float removing_value_co2_ = FIRST_REMOVING_VALUE_CO2_;//���� �߰�
    private const float FIRST_REMOVING_VALUE_CO2_ = 5; //�ִ�(ù) ���� �߰�
    private float adding_value_co2_ = FIRST_ADDING_VALUE_CO2_;//���� ����
    private const float FIRST_ADDING_VALUE_CO2_ = 10; //�ִ�(ù) ���� �߰�

    private float total_time_ = 120.0f; //��ü �ð�
    private float max_total_time_ = MAX_TOTAL_TIME_; //��ü �ð� �ƽ�
    private const float MAX_TOTAL_TIME_ = 120.0f; //���� ��ü �ð� �ƽ�
    private float round_time_ = 30.0f; //���� �ð�
    private float max_round_time_ = 30.0f; //���� �ð� �ƽ� (����)
    private const float FIRST_MAX_ROUND_TIME = 30.0f; //ù ���� �ð� �ƽ�
    private float remove_round_time_ = 0.5f;//���� ���� �ð�(����)
    private float FIRST_REMOVE_REOUND_TIME_ = 0.5f;//ù ���� ���� �ð�
    private float REMOVE_ROUND_TIME_RATE = 0.9f;//���� Ÿ�� ����ġ ���� ����

    private bool is_stop_round_time = false;

    private int perfect_count_ = 0; //����Ʈ ����
    private int perfect_count_fever_ = 0; //����Ʈ ���� (�ǹ� �뵵)
    private const int PERFECT_FEVER_COUNT_ = 5; //�ǹ� ������ ���� ����Ʈ ����
    private bool is_fever_ = false; //�ǹ� ��������
    private int remain_fever_count_ = 0; //�ǹ��� ���� �ߴ���
    private int max_fever_count_ = MAX_FEVER_COUNT_; //�ǹ� ����
    private const int MAX_FEVER_COUNT_ = 3; //���� �ǹ� ����

    public int BUTTON_COUNT = 4;

    private int tile_size_ = 5;
    public int MIN_TILE_SIZE_ = 5; //���� Ÿ�� ����
    public int MAX_TILE_SIZE_ = 10; //�ִ� Ÿ�� ����
    private int tile_count_ = MAX_TILE_COUNT_;
    private int max_tile_count_ = MAX_TILE_COUNT_; //�ִ� Ÿ�� ���� ī��Ʈ
    private const int MAX_TILE_COUNT_ = 3; //���� �ִ� Ÿ�� ���� ī��Ʈ

    private ArrowDirection[] tile_arrows_ = new ArrowDirection[20]; //ȭ��ǥ ����
    private int tile_index_ = 0; //���� Ÿ�� ��ġ

    public int LINE_TILES = 5; //�� �ٿ� ��� Ÿ���� �ִ���

    public static readonly int CAT_SIZE_ = 12;
    private bool[] using_cat_ = new bool[CAT_SIZE_]; //����̸� ������ΰ�
    private float[] using_cat_value_ = new float[CAT_SIZE_];//������� ��� ��ġ

    private const int DISRUPTOR_SIZE_ = 4;
    private bool[] using_disruptor_ = new bool[DISRUPTOR_SIZE_];  //� �����ڰ� ����Ǿ��°�?
    private bool use_disruptor_ = false; //�����ڰ� ���Ǵ°�?
    private float disruptor_probability_ = 0.2f; // ������ Ȯ��
    private int disruptor_count_ = 0;
    private int max_disruptor_count_ = 2; //�ǹ� �� ������ �ִ� ���� Ƚ��

    private const float DISRUPTOR_REMOVE_ROUND_TIME_RATE = 0.8f;
    private int disruptor_index_; //������ ��� �ε���
    private bool disruptor_round_check; //���忡 �����ڰ� ����Ǵ°�?


    [SerializeField]
    private bool[] ready_to_using_disruptors = new bool[4];
    [SerializeField]
    private float ready_to_disruptor_rate_;
    [SerializeField]
    private int ready_to_disruptor_count_;


    private bool disrutor_error_check_
    {
        get
        {
            if (!use_disruptor_) { Debug.Log("������ ��� ����"); return false; }

            bool using_check = false;
            for (int i = 0; i < DISRUPTOR_SIZE_; i++)
            {
                if (using_disruptor_[i] == true)
                {
                    using_check = true;
                    break;
                }
            }

            if (!using_check) { Debug.Log("����ϴ� �����ڰ� ����"); return false; }

            if (disruptor_probability_ <= 0.0f) { Debug.Log("Ȯ���� 0 ������"); return false; }

            if (max_disruptor_count_ < 1) { Debug.Log("�����ڰ� ���� �� �ִ� ���� 1 �̸���"); return false; }

            return true;
        }
    }

    private bool disruptor_hide_check_
    {
        get
        {
            return disruptor_round_check && disruptor_index_ == DisruptorIndex.HIDE_;
        }
    }
    private bool disruptor_timeremove_check_
    {
        get
        {
            return disruptor_round_check && disruptor_index_ == DisruptorIndex.TIME_REMOVE_;
        }
    }
    private bool disruptor_swap_check_
    {
        get
        {
            return disruptor_round_check && disruptor_index_ == DisruptorIndex.BUTTON_SWAP_;
        }

    }

    private bool disruptor_hardfever_check_
    {
        get
        {
            return disruptor_round_check && disruptor_index_ == DisruptorIndex.HARD_FEVER_;
        }

    }

    


    private void Awake()
    {
        is_started_ = false;

        for (int i = 0; i < CAT_SIZE_; i++)
        {
            using_cat_[i] = false;
        }

        DataManager.dataManager.requestSuccededDelegate += SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate += FailRequestEvent;

    }

    private void OnDestroy()
    {
        DataManager.dataManager.requestSuccededDelegate -= SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate -= FailRequestEvent;
    }

    private void Start()
    {
        int[] cat_idx = { -1, -1, -1 };
        float[] cat_value = { 0, 0, 0 };

        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            cat_idx[i] = DataManager.dataManager.GetSelectedCat(i);
            Debug.Log(i + "��°����� : " + cat_idx[i]);
            if (cat_idx[i] >= 0 && cat_idx[i] < CAT_SIZE_)
            {
                switch (cat_idx[i])
                {
                    case CatIndex.TOTAL_TIME_UP_:
                        cat_value[i] = CatValue.TOTAL_TIME_UP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.ROUND_TIME_UP_:
                        cat_value[i] = CatValue.ROUND_TIME_UP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.TILE_SPEED_DOWN_:
                        cat_value[i] = CatValue.TILE_SPEED_DOWN_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.LIFE_REMOVE_DOWN_:
                        cat_value[i] = CatValue.LIFE_REMOVE_DOWN_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.EXP_UP_:
                        cat_value[i] = CatValue.EXP_UP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.GOLD_UP_:
                        cat_value[i] = CatValue.GOLD_UP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.MISTAKE_DEFENCE_:
                        cat_value[i] = CatValue.MISTAKE_DEFENCE_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.FEVER_UP_:
                        cat_value[i] = CatValue.FEVER_UP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.BONUS_STAGE_:
                        cat_value[i] = CatValue.BONUS_STAGE_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.SIMPLE_LINE_:
                        cat_value[i] = CatValue.SIMPLE_LINE_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.TIME_STOP_:
                        cat_value[i] = CatValue.TIME_STOP_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    case CatIndex.SAVOTAGE_DEFENCE_:
                        cat_value[i] = CatValue.SAVOTAGE_DEFENCE_[DataManager.dataManager.GetLevelOfCat(i)];
                        break;
                    default:
                        break;
                }
            }
            else
            {
                cat_idx[i] = -1;
            }
        }

        SetUsingCat(cat_idx[0], cat_idx[1], cat_idx[2], cat_value[0], cat_value[1], cat_value[2]);
        visual_manager_.SetCatState(cat_idx);
        SetUsingDisruptor((ready_to_using_disruptors[0] || ready_to_using_disruptors[1] || ready_to_using_disruptors[2] || ready_to_using_disruptors[3]), 
            ready_to_using_disruptors[0], ready_to_using_disruptors[1], ready_to_using_disruptors[2], ready_to_using_disruptors[3], ready_to_disruptor_rate_, ready_to_disruptor_count_);


        visual_manager_.StartAnimationForStartGame();
    }

    private void Update()
    {
        if (is_started_) {
            SetTotalTimer(Time.deltaTime);
            SetRoundTimer(false,Time.deltaTime);
        }
    }

    private void ResetTiles() {
        FeverCheck();//�ǹ� üũ

        visual_manager_.ResetCatSkill();

        if (disruptor_swap_check_)
        {
            SwapButtons();
        }//������ �����ߴٸ� �ٽ� ����

        if (Mode == ChallangeMode.ButtonRoatator)
        {
            CHALLANGE_ButtonRotator.RotateButtons();
        }

        DisruptorCheck();//������ ��� üũ

        if (disruptor_round_check) { visual_manager_.ShowDisruptor(disruptor_index_); } //������ ��ų ��� ������


        if (disruptor_swap_check_)
        {
            SwapButtons();
            Debug.Log("��ư ����!");
        }

        tile_index_ = 0;

        if (tile_size_ != MAX_TILE_SIZE_) {
            tile_count_--;
            if (tile_count_ <= 0) {
                tile_size_++;
                tile_count_ = max_tile_count_;
            }
        }

        

        if (is_fever_)
        {
            visual_manager_.SetCatAnimationFever(); //�ӽ�

            if (using_cat_[CatIndex.FEVER_UP_] && remain_fever_count_ <= max_fever_count_ - MAX_FEVER_COUNT_)
            {
                visual_manager_.PlayCatSkill((int)CatIndex.FEVER_UP_);
            }

            if (disruptor_hardfever_check_)
            {
                Debug.Log("�ϵ� �ǹ�!");

                ArrowDirection dir1 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                ArrowDirection dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);

                while (dir1 == dir2)
                {
                    dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                }
                for (int i = 0; i < tile_size_; i++)
                {
                    int idx = Random.Range(0, 2);
                    if (idx == 0)
                    {
                        tile_manager_.SetState(true, i, dir1, disruptor_hide_check_);
                        tile_arrows_[i] = dir1;
                    }
                    else
                    {
                        tile_manager_.SetState(true, i, dir2, disruptor_hide_check_);
                        tile_arrows_[i] = dir2;
                    }
                }
            }
            else
            {
                Debug.Log("�ǹ�!");
                ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                for (int i = 0; i < tile_size_; i++)
                {
                    tile_manager_.SetState(true, i, dir);
                    tile_arrows_[i] = dir;
                }
            }
        }
        else {
            if (using_cat_[CatIndex.BONUS_STAGE_] && using_cat_[CatIndex.SIMPLE_LINE_])
            {
                int choice = Random.Range(1, 3);
                if (choice == 1)
                {
                    int range = Mathf.RoundToInt(using_cat_value_[CatIndex.BONUS_STAGE_] * 100.0f);
                    int num = Random.Range(1, 101);
                    if (range >= num)
                    {
                        Debug.Log("���ʽ� ��������!");

                        visual_manager_.PlayCatSkill((int)CatIndex.BONUS_STAGE_);

                        ArrowDirection dir1 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                        ArrowDirection dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);

                        while (dir1 == dir2)
                        {
                            dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                        }
                        for (int i = 0; i < tile_size_; i++)
                        {
                            int idx = Random.Range(0, 2);
                            if (idx == 0)
                            {
                                tile_manager_.SetState(true, i, dir1, disruptor_hide_check_);
                                tile_arrows_[i] = dir1;
                            }
                            else
                            {
                                tile_manager_.SetState(true, i, dir2, disruptor_hide_check_);
                                tile_arrows_[i] = dir2;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tile_size_; i++)
                        {
                            ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                            tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                            tile_arrows_[i] = dir;
                        }
                    }
                }
                else {
                    int range = Mathf.RoundToInt(using_cat_value_[CatIndex.SIMPLE_LINE_] * 100.0f);
                    int num = Random.Range(1, 101);
                    if (range >= num)
                    {
                        Debug.Log("�ܼ�ȭ!");


                        ArrowDirection dir_simple = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                        int line = Random.Range(0, tile_size_ / LINE_TILES);

                        visual_manager_.PlayCatSkill((int)CatIndex.SIMPLE_LINE_, true, line);

                        for (int i = 0; i < tile_size_; i++)
                        {
                            int idx = Random.Range(0, 2);
                            if (i >= line * LINE_TILES && i < (line + 1) * LINE_TILES)
                            {
                                tile_manager_.SetState(true, i, dir_simple, disruptor_hide_check_);
                                tile_arrows_[i] = dir_simple;
                            }
                            else
                            {
                                ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                                tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                                tile_arrows_[i] = dir;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tile_size_; i++)
                        {
                            ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                            tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                            tile_arrows_[i] = dir;
                        }
                    }
                }

            }
            else if (using_cat_[CatIndex.BONUS_STAGE_])
            {
                int range = Mathf.RoundToInt(using_cat_value_[CatIndex.BONUS_STAGE_] * 100.0f);
                int num = Random.Range(1, 101);
                if (range >= num)
                {
                    Debug.Log("���ʽ� ��������!");

                    visual_manager_.PlayCatSkill((int)CatIndex.BONUS_STAGE_);

                    ArrowDirection dir1 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                    ArrowDirection dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);

                    while (dir1 == dir2)
                    {
                        dir2 = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                    }
                    for (int i = 0; i < tile_size_; i++)
                    {
                        int idx = Random.Range(0, 2);
                        if (idx == 0)
                        {
                            tile_manager_.SetState(true, i, dir1, disruptor_hide_check_);
                            tile_arrows_[i] = dir1;
                        }
                        else
                        {
                            tile_manager_.SetState(true, i, dir2, disruptor_hide_check_);
                            tile_arrows_[i] = dir2;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tile_size_; i++)
                    {
                        ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                        tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                        tile_arrows_[i] = dir;
                    }
                }
            }
            else if (using_cat_[CatIndex.SIMPLE_LINE_])
            {
                int range = Mathf.RoundToInt(using_cat_value_[CatIndex.SIMPLE_LINE_] * 100.0f);
                int num = Random.Range(1, 101);
                if (range >= num)
                {
                    Debug.Log("�ܼ�ȭ!");


                    ArrowDirection dir_simple = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                    int line = Random.Range(0, tile_size_ / LINE_TILES);

                    visual_manager_.PlayCatSkill((int)CatIndex.SIMPLE_LINE_,true,line);

                    for (int i = 0; i < tile_size_; i++)
                    {
                        int idx = Random.Range(0, 2);
                        if (i >= line * LINE_TILES && i < (line + 1) * LINE_TILES)
                        {
                            tile_manager_.SetState(true, i, dir_simple, disruptor_hide_check_);
                            tile_arrows_[i] = dir_simple;
                        }
                        else
                        {
                            ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                            tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                            tile_arrows_[i] = dir;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tile_size_; i++)
                    {
                        ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                        tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                        tile_arrows_[i] = dir;
                    }
                }
            }
            else {
                for (int i = 0; i < tile_size_; i++)
                {
                    ArrowDirection dir = (ArrowDirection)Random.Range(0, BUTTON_COUNT);
                    tile_manager_.SetState(true, i, dir, disruptor_hide_check_);
                    tile_arrows_[i] = dir;
                }
            }
        }

        SetRoundTimer(true);
        is_perfect_ = true;
    }


    private void IncreaseTileIndex() {
        tile_index_++;
        if (tile_index_ == tile_size_)
        {
            if (is_perfect_)
            {
                perfect_count_++;
                AddScore(ADDING_SCORE_TILE_);
                RemoveCO2(removing_value_co2_);
                visual_manager_.SetCatAnimation(true); //�ӽ�
            }
            ResetTiles();
        }
        else if (disruptor_hide_check_)
        {
            tile_manager_.SetState(true, tile_index_, tile_arrows_[tile_index_]);
        }
    }

    private void FeverCheck() {
        if (is_fever_)
        {
            {
                remain_fever_count_--;
                if (remain_fever_count_ == 0)
                {
                    visual_manager_.EndFever();
                    is_fever_ = false;
                    disruptor_count_ = 0;
                }
            }
        }
        else
        {
            if (is_perfect_) {
                perfect_count_fever_++;
            }

            if (perfect_count_fever_ == PERFECT_FEVER_COUNT_)
            {
                visual_manager_.StartFever();
                perfect_count_fever_ = 0;
                is_fever_ = true;
                remain_fever_count_ = max_fever_count_;
            }
        }
    }

    private void DisruptorCheck() {
        if (use_disruptor_ && disruptor_count_ < max_disruptor_count_)
        {
            int range = Mathf.RoundToInt(disruptor_probability_ * 100.0f);
            int num = Random.Range(1, 101);
            if (range >= num)
            {
                if (using_cat_[CatIndex.SAVOTAGE_DEFENCE_]) { 
                    int def_range = Mathf.RoundToInt(using_cat_value_[CatIndex.SAVOTAGE_DEFENCE_] * 100.0f);
                    int def_num = Random.Range(1, 101);
                    if (def_range >= def_num)
                    {
                        Debug.Log("����� : ������ ���� ����");
                        visual_manager_.PlayCatSkill((int)CatIndex.SAVOTAGE_DEFENCE_);
                        disruptor_count_++;
                        disruptor_round_check = false;
                        return;
                    }

                }


                int idx;

                if (is_fever_)
                {
                    Debug.Log("������ : �ǹ���");
                    if (using_disruptor_[DisruptorIndex.HARD_FEVER_])
                    {
                        idx = DisruptorIndex.HARD_FEVER_;
                        Debug.Log("������ : �ϵ��ǹ� ����");
                    }
                    else {
                        disruptor_round_check = false; 
                        return;
                    }
                }
                else
                {
                    Debug.Log("������ : �ǹ��ƴ�");
                    if (using_disruptor_[DisruptorIndex.HARD_FEVER_] && !using_disruptor_[DisruptorIndex.BUTTON_SWAP_] && !using_disruptor_[DisruptorIndex.TIME_REMOVE_] && !using_disruptor_[DisruptorIndex.HIDE_])
                    {
                        Debug.Log("������ : �ǹ��ۿ� ��� ����...");
                        disruptor_round_check = false;
                        return;
                    }
                    else
                    {
                        idx = Random.Range(0, DISRUPTOR_SIZE_);
                        while (!using_disruptor_[idx] || idx == DisruptorIndex.HARD_FEVER_)
                        {
                            idx = Random.Range(0, DISRUPTOR_SIZE_);
                        }
                        Debug.Log("������ : �Ϲݴܰ� ���� : " + idx + "���õ�");
                    }
                }
                disruptor_count_++;
                disruptor_round_check = true;
                disruptor_index_ = idx;
            }
            else
            {
                disruptor_round_check = false;
            }
        }
        else
        {
            disruptor_round_check = false;
        }

    }


    private void AddScore(int val) {
        if (is_fever_)
        {
            if (val >= 0) { score_ += Mathf.RoundToInt(val * 1.5f); }
        }
        else {
            score_ += val;
        }
        if (score_ < 0) { score_ = 0; }
        ScoreText.text = score_.ToString();

    }

    private void RemoveCO2(float val)
    {
        co2_ -= val;
        if (co2_ < 0) { co2_ = 0; }
        else if (co2_ > MAX_CO2_) { co2_ = MAX_CO2_; }
        visual_manager_.SetCo2Value(co2_ / MAX_CO2_);

        layer_manager_.SetLayer(co2_ / MAX_CO2_);

        if (co2_ == MAX_CO2_) {
            EndGame();
        }
    }

    private void AddCO2(float val)
    {
        co2_ += val;
        if (co2_ < 0) { co2_ = 0; }
        else if (co2_ > MAX_CO2_) { co2_ = MAX_CO2_; }
        visual_manager_.SetCo2Value(co2_ / MAX_CO2_);

        layer_manager_.SetLayer(co2_ / MAX_CO2_);

        if (co2_ == MAX_CO2_)
        {
            EndGame();
        }
    }

    public void SetUsingCat(int num1, int num2, int num3, float lev1=1, float lev2 = 1, float lev3 = 1)
    {
        if (num1 >= 0 && num1 <= CAT_SIZE_)
        {
            using_cat_[num1] = true;
            using_cat_value_[num1] = lev1;
        }

        if (num2 >= 0 && num2 <= CAT_SIZE_ && num1 != num2)
        {
            using_cat_[num2] = true;
            using_cat_value_[num2] = lev2;
        }

        if (num3 >= 0 && num3 <= CAT_SIZE_ && num1 != num3 && num2 != num3)
        {
            using_cat_[num3] = true;
            using_cat_value_[num3] = lev3;
        }
    } //�ܺο��� ����� ����� ����

    public void SetUsingDisruptor(bool use, bool use_dis1=false, bool use_dis2 = false, bool use_dis3 = false, bool use_dis4 = false, float probability=0.1f, int max_disruptor_count=2)
    {
        use_disruptor_ = use;

        if (use)
        {
            using_disruptor_[0] = use_dis1;
            using_disruptor_[1] = use_dis2;
            using_disruptor_[2] = use_dis3;
            using_disruptor_[3] = use_dis4;

            disruptor_probability_ = probability;
            max_disruptor_count_ = max_disruptor_count;
            disruptor_count_ = 0;

            if (!disrutor_error_check_) { 
                use_disruptor_ = false; 
                Debug.Log("�����Ϳ� ���� �߻� : ������ �ȳ�Ÿ���� ����"); 
            }
        }
    } //�ܺο��� ����� ������ ����

    private void SetTotalTimer(float delta_time = 0.0f) {
        total_time_ -= delta_time;

        if (total_time_ <= 0.0f)
        {
            total_time_ = 0.0f;
            TotalTimerSlider.value = total_time_ / max_total_time_;

            EndGame();
        }
        else {
            TotalTimerSlider.value = total_time_ / max_total_time_;
        }
    }

    private void SetRoundTimer(bool is_reset, float delta_time = 0.0f) {
        if (is_reset)
        {
            if (using_cat_[CatIndex.TIME_STOP_])
            {
                int range = Mathf.RoundToInt(using_cat_value_[CatIndex.TIME_STOP_] * 100.0f);
                int num = Random.Range(1, 101);
                if (range >= num)
                {
                    Debug.Log("�ð� ����!");
                    visual_manager_.PlayCatSkill((int)CatIndex.TIME_STOP_);
                    is_stop_round_time = true;
                }
                else 
                {
                    is_stop_round_time = false;
                }
            }


            max_round_time_ -= remove_round_time_;
            remove_round_time_ *= REMOVE_ROUND_TIME_RATE;
            round_time_ = max_round_time_;

            if (disruptor_timeremove_check_)
            {
                round_time_ *= DISRUPTOR_REMOVE_ROUND_TIME_RATE;
                Debug.Log("������ : �ð� ����!");
            }

            RoundTimerSlider.value = round_time_ / max_round_time_;
        }
        else {
            if (is_stop_round_time) 
            {
                return;
            }

            round_time_ -= delta_time;

            if (round_time_ <= 0.0f)
            {
                round_time_ = 0.0f;
                RoundTimerSlider.value = round_time_ / max_round_time_;

                is_perfect_ = false;
                ResetTiles();
                AddCO2(adding_value_co2_ * 2.0f);
            }
            else {
                RoundTimerSlider.value = round_time_ / max_round_time_;
            }
        }
    }

    private void GetReadyTimers() {

        total_time_ = max_total_time_;
        round_time_ = max_round_time_;

        remove_round_time_ = FIRST_REMOVE_REOUND_TIME_;

        TotalTimerSlider.value = total_time_ / MAX_TOTAL_TIME_;
        RoundTimerSlider.value = round_time_ / max_round_time_;
    }

    public void ReturnToLobby() {
        LoadingManager.LoadScene("LobbyScene");
    }

    public void StartGame() {
        if (!is_started_)
        {
            is_started_ = true;

            tile_size_ = MIN_TILE_SIZE_;

            score_ = 0;
            ScoreText.text = "0";

            is_swap_ = false;
            is_perfect_ = false;
            is_fever_ = false;
            is_stop_round_time = false;

            perfect_count_ = 0;
            perfect_count_fever_ = 0;

            co2_ = MAX_CO2_ / 2.0f;
            layer_manager_.SetLayer(co2_ / MAX_CO2_);
            visual_manager_.SetCo2Value(co2_ / MAX_CO2_);

            removing_value_co2_ = FIRST_REMOVING_VALUE_CO2_;
            adding_value_co2_ = FIRST_ADDING_VALUE_CO2_;
            max_total_time_ = MAX_TOTAL_TIME_;
            max_round_time_ = FIRST_MAX_ROUND_TIME;
            max_tile_count_ = MAX_TILE_COUNT_;
            max_fever_count_ = MAX_FEVER_COUNT_;

            SetStartCatSkill();

            tile_count_ = max_tile_count_ + 1; //ResetTiles()���� �ѹ� �����ϰ� ������ ���̱� ������ +1
            GetReadyTimers();

            ResetTiles();

            bgm_manager_.PlayBGM();
        }
    }

    private void EndGame() {
        if (is_started_)
        {
            is_started_ = false;

            int gold = score_ / DataManager.COIN_PER_SCORE;
            if (using_cat_[CatIndex.GOLD_UP_])
            {
                gold = Mathf.RoundToInt(gold*using_cat_value_[CatIndex.GOLD_UP_]);
            }

            int ex = score_ / DataManager.EXP_PER_SCORE;
            if (using_cat_[CatIndex.EXP_UP_])
            {
                ex = Mathf.RoundToInt(ex * using_cat_value_[CatIndex.EXP_UP_]);
            }

            DataManager.dataManager.UpdateScore(score_,stageID);
            visual_manager_.StartAnimationForEndGame(score_, gold, ex, using_cat_[CatIndex.GOLD_UP_], using_cat_[CatIndex.EXP_UP_]);
            Debug.Log("��������...");
        }
    }


    public void OnClickButton(ArrowDirection dir)
    {
        if (is_started_)
        {
            if (dir == tile_arrows_[tile_index_])
            {
                tile_manager_.PopTile(tile_index_,true);
                AddScore(ADDING_SCORE_TILE_);
                IncreaseTileIndex();
            }
            else
            {
                if (using_cat_[CatIndex.MISTAKE_DEFENCE_] && using_cat_value_[CatIndex.MISTAKE_DEFENCE_] > 0)
                {
                    Debug.Log("�Ǽ� ����!");
                    visual_manager_.PlayCatSkill((int)CatIndex.MISTAKE_DEFENCE_);
                    using_cat_value_[CatIndex.MISTAKE_DEFENCE_] -= 1;
                    tile_manager_.PopTile(tile_index_, true);
                    IncreaseTileIndex();
                }
                else
                {
                    if (Mode == ChallangeMode.OneLife) {
                        EndGame();
                    }

                    is_perfect_ = false;

                    tile_manager_.PopTile(tile_index_, false);
                    visual_manager_.SetCatAnimation(false); 

                    AddScore(REMOVING_SCORE_TILE_);
                    AddCO2(adding_value_co2_);
                    if (co2_ >= MAX_CO2_)
                    {
                        return;
                    }
                    IncreaseTileIndex();
                }
            }
        }
    }


    private void SetStartCatSkill() {

        bool isPlaySkiilSound = true;

        if (using_cat_[CatIndex.TOTAL_TIME_UP_]) 
        {
            visual_manager_.PlayCatSkill((int)CatIndex.TOTAL_TIME_UP_, isPlaySkiilSound);
            max_total_time_ = MAX_TOTAL_TIME_ + using_cat_value_[CatIndex.TOTAL_TIME_UP_];
            Debug.Log("���� �ð� ����! : " + max_total_time_.ToString());
            isPlaySkiilSound = false;
        }

        if (using_cat_[CatIndex.ROUND_TIME_UP_])
        {
            visual_manager_.PlayCatSkill((int)CatIndex.ROUND_TIME_UP_, isPlaySkiilSound);
            max_round_time_ = FIRST_MAX_ROUND_TIME + using_cat_value_[CatIndex.ROUND_TIME_UP_];
            Debug.Log("���� �ð� ����! : " + max_round_time_.ToString());
            isPlaySkiilSound = false;
        }

        if (using_cat_[CatIndex.TILE_SPEED_DOWN_])
        {
            visual_manager_.PlayCatSkill((int)CatIndex.TILE_SPEED_DOWN_, isPlaySkiilSound);
            max_tile_count_ = MAX_TILE_COUNT_ + Mathf.RoundToInt(using_cat_value_[CatIndex.TILE_SPEED_DOWN_]);
            Debug.Log("Ÿ�� ������ ����! : " + max_tile_count_.ToString());
            isPlaySkiilSound = false;
        }

        if (using_cat_[CatIndex.LIFE_REMOVE_DOWN_])
        {
            visual_manager_.PlayCatSkill((int)CatIndex.LIFE_REMOVE_DOWN_, isPlaySkiilSound);
            adding_value_co2_ = FIRST_ADDING_VALUE_CO2_ - using_cat_value_[CatIndex.LIFE_REMOVE_DOWN_];
            Debug.Log("������ ����! : " + adding_value_co2_.ToString());
            isPlaySkiilSound = false;
        }

        if (using_cat_[CatIndex.FEVER_UP_])
        {
            max_fever_count_ = MAX_FEVER_COUNT_ + Mathf.RoundToInt(using_cat_value_[CatIndex.FEVER_UP_]);
        }
    }


    private void SwapButtons() {
        is_swap_ = !is_swap_;
        ButtonSwapAnimator.SetBool(SWAP_BUTTONS_HASH, is_swap_);
    }



    private void SuccessRequestEvent()
    {
        visual_manager_.ShowEndButton();

    }

    private void FailRequestEvent(string err)
    {
        visual_manager_.ShowErrorMessage();
        visual_manager_.ShowEndButton();
    }


}
