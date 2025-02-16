using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ArrowDirection { LU, LD, RU, RD }

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
    }//GameManager를 싱글턴으로 변경

    private static GameManager gameManager_instance; //싱글턴 인스턴스


    public TMP_Text ScoreText; //스코어 텍스트 : 외부 지정
    public Image LifeImage;
    public Image TotalTimerImage;
    public Image RoundTimerImage;



    [SerializeField]
    private TileManager tile_manager_;

    private bool is_started_ = false; //게임 시작 유무

    private bool is_perfect_ = true; //퍼펙트 유무

    private int score_ = 0; //점수
    private const int ADDING_SCORE_TILE_= 10; //타일 점수
    private const int ADDING_SCORE_PERFECT_ = 20; //퍼펙트 점수
    private const int REMOVING_SCORE_TILE_ = -5; //감점

    private float life_ = 100; //생명
    private const float MAX_LIFE_ = 100; //생명 최대값
    private const float REMOVING_VALUE_LIFE_ = 10; //생명 감점

    private float total_time_ = 100.0f; //전체 시간
    private const float MAX_TOTAL_TIME_ = 100.0f; //전체 시간 맥스
    private float round_time_ = 20.0f; //라운드 시간
    private float max_round_time_ = 20.0f; //라운드 시간 맥스 (변동)
    private const float FIRST_MAX_ROUND_TIME = 20.0f; //첫 라운드 시간 맥스
    private float remove_round_time_ = 0.5f;//라운드 감소 시간(변동)
    private float FIRST_REMOVE_REOUND_TIME_ = 0.5f;//첫 라운드 감소 시간
    private float REMOVE_ROUND_TIME_RATE = 0.9f;//라운드 타임 감소치 감소 비율

    private int perfect_count_ = 0; //퍼펙트 개수
    private int perfect_count_fever_ = 0; //퍼펙트 개수 (피버 용도)
    private const int PERFECT_FEVER_COUNT_ = 5; //피버 도달을 위한 퍼펙트 개수
    private bool is_fever = false; //피버 상태인지
    private int remain_fever_count = 0; //피버에 도달 했는지
    private const int MAX_FEVER_COUNT = 2; //피버 개수

    public const int tile_size_ = 10; //타일 개수
    private ArrowDirection[] tile_arrows_ = new ArrowDirection[10]; //화살표 방향
    private int tile_index_ = 0; //현재 타일 위치

    

    private void Awake()
    {
        is_started_ = false;
    }

    private void Start()
    {
        StartGame(); //!!!!임시코드 : 삭제 할 예정
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
        Debug.Log("피버카운트 : " + perfect_count_fever_);
        tile_index_ = 0;

        if (is_fever)
        {
            Debug.Log("피버!");
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
        if (tile_index_ == 10) {
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

    private void SetTotalTimer(float delta_time = 0.0f) {
        total_time_ -= delta_time;

        if (total_time_ <= 0.0f)
        {
            total_time_ = 0.0f;
            TotalTimerImage.fillAmount = total_time_ / MAX_TOTAL_TIME_;

            EndGame();
        }
        else {
            TotalTimerImage.fillAmount = total_time_ / MAX_TOTAL_TIME_;
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
                RemoveLife(REMOVING_VALUE_LIFE_ * 2);
            }
            else {
                RoundTimerImage.fillAmount = round_time_ / max_round_time_;
            }
        }
    }

    private void GetReadyTimers() {
        total_time_ = MAX_TOTAL_TIME_;
        max_round_time_ = FIRST_MAX_ROUND_TIME;
        remove_round_time_ = FIRST_REMOVE_REOUND_TIME_;
        round_time_ = max_round_time_;

        TotalTimerImage.fillAmount = total_time_ / MAX_TOTAL_TIME_;
        RoundTimerImage.fillAmount = round_time_ / max_round_time_;
    }

    private void StartGame() {
        if (!is_started_)
        {
            score_ = 0;
            is_perfect_ = false;
            is_started_ = true;
            is_fever = false;
            perfect_count_ = 0;
            perfect_count_fever_ = 0;

            GetReadyTimers();
            ResetTiles();
        }
    }

    private void EndGame() {
        if (is_started_)
        {
            is_started_ = false;
            Debug.Log("게임종료...");
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
                RemoveLife(REMOVING_VALUE_LIFE_);
                if (life_ <= 0.0f) {
                    return;
                }
                IncreaseTileIndex();
            }
        }
    }
}
