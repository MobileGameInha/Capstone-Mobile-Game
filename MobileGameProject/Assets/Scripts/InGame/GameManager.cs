using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public enum ArrowDirection { LU, LD, RU, RD }


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
    public Image LifeImage;
    public Image TotalTimerImage;
    public Image RoundTimerImage;



    [SerializeField]
    private TileManager tile_manager_;

    private bool is_started_ = false; //���� ���� ����

    private bool is_perfect_ = true; //����Ʈ ����

    private int score_ = 0; //����
    private const int ADDING_SCORE_TILE_= 10; //Ÿ�� ����
    private const int ADDING_SCORE_PERFECT_ = 20; //����Ʈ ����
    private const int REMOVING_SCORE_TILE_ = -5; //����

    private float life_ = 100; //����
    private const float MAX_LIFE_ = 100; //���� �ִ밪
    private float removing_value_life_ = FIRST_REMOVING_VALUE_LIFE_;//���� ����
    private const float FIRST_REMOVING_VALUE_LIFE_ = 10; //�ִ�(ù) ���� ����

    private float total_time_ = 100.0f; //��ü �ð�
    private float max_total_time_ = MAX_TOTAL_TIME_; //��ü �ð� �ƽ�
    private const float MAX_TOTAL_TIME_ = 100.0f; //���� ��ü �ð� �ƽ�
    private float round_time_ = 20.0f; //���� �ð�
    private float max_round_time_ = 20.0f; //���� �ð� �ƽ� (����)
    private const float FIRST_MAX_ROUND_TIME = 20.0f; //ù ���� �ð� �ƽ�
    private float remove_round_time_ = 0.5f;//���� ���� �ð�(����)
    private float FIRST_REMOVE_REOUND_TIME_ = 0.5f;//ù ���� ���� �ð�
    private float REMOVE_ROUND_TIME_RATE = 0.9f;//���� Ÿ�� ����ġ ���� ����

    private int perfect_count_ = 0; //����Ʈ ����
    private int perfect_count_fever_ = 0; //����Ʈ ���� (�ǹ� �뵵)
    private const int PERFECT_FEVER_COUNT_ = 5; //�ǹ� ������ ���� ����Ʈ ����
    private bool is_fever = false; //�ǹ� ��������
    private int remain_fever_count = 0; //�ǹ��� ���� �ߴ���
    private const int MAX_FEVER_COUNT = 2; //�ǹ� ����

    private int tile_size_ = MIN_TILE_SIZE_;
    public const int MIN_TILE_SIZE_ = 5; //���� Ÿ�� ����
    public const int MAX_TILE_SIZE_ = 10; //�ִ� Ÿ�� ����
    private int tile_count_ = MAX_TILE_COUNT_;
    public int max_tile_count_ = MAX_TILE_COUNT_; //�ִ� Ÿ�� ����
    public const int MAX_TILE_COUNT_ = 3; //���� �ִ� Ÿ�� ����

    private ArrowDirection[] tile_arrows_ = new ArrowDirection[10]; //ȭ��ǥ ����
    private int tile_index_ = 0; //���� Ÿ�� ��ġ


    private const int CAT_SIZE_ = 12;
    private bool[] using_cat_ = new bool[CAT_SIZE_]; //����̸� ������ΰ�
    private float[] using_cat_value_ = new float[CAT_SIZE_];//������� ��� ��ġ

    private void Awake()
    {
        is_started_ = false;

        for (int i = 0; i < CAT_SIZE_; i++)
        {
            using_cat_[i] = false;
        }
    }

    private void Start()
    {
        SetUsingCat(3,-1,-1,5); //!!!!�ӽ��ڵ� : ���� �� ����
        StartGame(); //!!!!�ӽ��ڵ� : ���� �� ����
    }

    private void Update()
    {
        if (is_started_) {
            SetTotalTimer(Time.deltaTime);
            SetRoundTimer(false,Time.deltaTime);
        }
    }

    private void ResetTiles() {
        FeverCheck();
        Debug.Log("�ǹ�ī��Ʈ : " + perfect_count_fever_);
        tile_index_ = 0;

        if (tile_size_ != MAX_TILE_SIZE_) {
            tile_count_--;
            if (tile_count_ <= 0) {
                tile_size_++;
                tile_count_ = max_tile_count_;
            }
        }

        if (is_fever)
        {
            Debug.Log("�ǹ�!");
            ArrowDirection dir = (ArrowDirection)Random.Range(0, 4);
            for (int i = 0; i < tile_size_; i++)
            {
                tile_manager_.SetState(true, i, dir);
                tile_arrows_[i] = dir;
            }
        }
        else { 
            for (int i = 0; i < tile_size_; i++)
            {
            ArrowDirection dir = (ArrowDirection)Random.Range(0, 4);
            tile_manager_.SetState(true, i, dir);
            tile_arrows_[i] = dir;
            }
        }

        SetRoundTimer(true);
        is_perfect_ = true;
    }


    private void IncreaseTileIndex() {
        tile_index_++;
        if (tile_index_ == tile_size_) {
            if (is_perfect_) {
                perfect_count_++;
                AddScore(ADDING_SCORE_TILE_);
            }
            ResetTiles();
        }
    }

    private void FeverCheck() {
        if (is_fever)
        {
            {
                remain_fever_count--;
                if (remain_fever_count == 0)
                {
                    is_fever = false;
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
                perfect_count_fever_ = 0;
                is_fever = true;
                remain_fever_count = MAX_FEVER_COUNT;
            }
        }
    }

    private void AddScore(int val) {
        if (is_fever)
        {
            if (val >= 0) { score_ += Mathf.RoundToInt(val * 1.5f); }
        }
        else {
            score_ += val;
        }
        if (score_ < 0) { score_ = 0; }
        ScoreText.text = score_.ToString();
    }

    private void RemoveLife(float val)
    {
        life_ -= val;
        if (life_ < 0) { life_ = 0; }
        LifeImage.fillAmount = life_ / MAX_LIFE_;

        if (life_ == 0.0f) {
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

    private void SetTotalTimer(float delta_time = 0.0f) {
        total_time_ -= delta_time;

        if (total_time_ <= 0.0f)
        {
            total_time_ = 0.0f;
            TotalTimerImage.fillAmount = total_time_ / max_total_time_;

            EndGame();
        }
        else {
            TotalTimerImage.fillAmount = total_time_ / max_total_time_;
        }
    }

    private void SetRoundTimer(bool is_reset, float delta_time = 0.0f) {
        if (is_reset)
        {
            max_round_time_ -= remove_round_time_;
            remove_round_time_ *= REMOVE_ROUND_TIME_RATE;
            round_time_ = max_round_time_;

            RoundTimerImage.fillAmount = round_time_ / max_round_time_;
        }
        else {
            round_time_ -= delta_time;

            if (round_time_ <= 0.0f)
            {
                round_time_ = 0.0f;
                RoundTimerImage.fillAmount = round_time_ / max_round_time_;

                is_perfect_ = false;
                ResetTiles();
                RemoveLife(removing_value_life_ * 2);
            }
            else {
                RoundTimerImage.fillAmount = round_time_ / max_round_time_;
            }
        }
    }

    private void GetReadyTimers() {

        total_time_ = max_total_time_;
        round_time_ = max_round_time_;

        remove_round_time_ = FIRST_REMOVE_REOUND_TIME_;

        TotalTimerImage.fillAmount = total_time_ / MAX_TOTAL_TIME_;
        RoundTimerImage.fillAmount = round_time_ / max_round_time_;
    }

    private void StartGame() {
        if (!is_started_)
        {
            is_started_ = true;

            tile_size_ = MIN_TILE_SIZE_;

            score_ = 0;

            is_perfect_ = false;
            is_fever = false;

            perfect_count_ = 0;
            perfect_count_fever_ = 0;

            removing_value_life_ = FIRST_REMOVING_VALUE_LIFE_;
            max_total_time_ = MAX_TOTAL_TIME_;
            max_round_time_ = FIRST_MAX_ROUND_TIME;
            max_tile_count_ = MAX_TILE_COUNT_;

            SetStartCatSkill();

            tile_count_ = max_tile_count_ + 1; //ResetTiles()���� �ѹ� �����ϰ� ������ ���̱� ������ +1
            GetReadyTimers();

            ResetTiles();
        }
    }

    private void EndGame() {
        if (is_started_)
        {
            is_started_ = false;
            Debug.Log("��������...");
        }
    }


    public void OnClickButton(ArrowDirection dir)
    {
        if (is_started_)
        {
            if (dir == tile_arrows_[tile_index_])
            {
                tile_manager_.SetState(false, tile_index_);
                AddScore(ADDING_SCORE_TILE_);
                IncreaseTileIndex();
            }
            else
            {
                is_perfect_ = false;

                tile_manager_.SetState(false, tile_index_);
                AddScore(REMOVING_SCORE_TILE_);
                RemoveLife(removing_value_life_);
                if (life_ <= 0.0f) {
                    return;
                }
                IncreaseTileIndex();
            }
        }
    }


    private void SetStartCatSkill() {

        if (using_cat_[CatIndex.TOTAL_TIME_UP_]) 
        {
            max_total_time_ = MAX_TOTAL_TIME_ + using_cat_value_[CatIndex.TOTAL_TIME_UP_];
            Debug.Log("���� �ð� ����! : " + max_total_time_.ToString());
        }

        if (using_cat_[CatIndex.ROUND_TIME_UP_])
        {
            max_round_time_ = FIRST_MAX_ROUND_TIME + using_cat_value_[CatIndex.ROUND_TIME_UP_];
            Debug.Log("���� �ð� ����! : " + max_round_time_.ToString());
        }

        if (using_cat_[CatIndex.TILE_SPEED_DOWN_])
        {
            max_tile_count_ = MAX_TILE_COUNT_ + Mathf.RoundToInt(using_cat_value_[CatIndex.TILE_SPEED_DOWN_]);
            Debug.Log("Ÿ�� ������ ����! : " + max_tile_count_.ToString());
        }

        if (using_cat_[CatIndex.LIFE_REMOVE_DOWN_])
        {
            removing_value_life_ = FIRST_REMOVING_VALUE_LIFE_ - using_cat_value_[CatIndex.LIFE_REMOVE_DOWN_];
            Debug.Log("������ ����! : " + removing_value_life_.ToString());
        }

    }







    

}
