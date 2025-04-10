using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager dataManager
    {
        get
        {
            if (dataManager_instance == null)
            {
                dataManager_instance = FindObjectOfType<DataManager>();
            }

            return dataManager_instance;
        }
    }//DataManager를 싱글턴으로 변경

    private static DataManager dataManager_instance; //싱글턴 인스턴스


    private const float MAX_EXP = 500.0f;
    private const float MAX_CAT_EXP = 500.0f;

    private string nickname_ = "Player";
    private int coin_=0;
    private float exp_ = 0.0f;

    [SerializeField] //삭제 예정
    private int[] selected_cat_ = { -1,-1,-1};
    private bool[] is_unlock_cat_;
    private int[] level_cat_;
    private float[] exp_cat_;



    public bool SetSelectedCat(int idx, int cat_idx)
    {
        if (cat_idx>=0 && cat_idx<GameManager.CAT_SIZE_&&is_unlock_cat_[cat_idx])
        {
            for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
            {
                if (selected_cat_[i] == cat_idx)
                {
                    selected_cat_[i] = -1;
                }
            }
            selected_cat_[idx] = cat_idx;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetIsUnlockCat(int idx)
    {
        return is_unlock_cat_[idx];
    }
    public int GetSelectedCat(int idx)
    {

        if (selected_cat_[idx]!=-1 && is_unlock_cat_[selected_cat_[idx]])
        {
            return selected_cat_[idx];
        }
        else 
        {
            return -1;
        }
    }

    public int GetLevelOfCat(int idx)
    {
        return level_cat_[idx];
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);

        selected_cat_ = new int[BasicHelperManager.MAX_HELPER_];
        is_unlock_cat_ = new bool[GameManager.CAT_SIZE_];
        level_cat_ = new int[GameManager.CAT_SIZE_];
        exp_cat_ = new float[GameManager.CAT_SIZE_];

        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            selected_cat_[i] = -1;
        }

        for (int i = 0; i < GameManager.CAT_SIZE_; i++)
        {
            is_unlock_cat_[i] = false;
            level_cat_[i] = 0;
            exp_cat_[i] = 0;
        }






        //!!!!임시코드 고양이 지정
        is_unlock_cat_[CatIndex.TOTAL_TIME_UP_] = true;
        is_unlock_cat_[CatIndex.ROUND_TIME_UP_] = true;
        is_unlock_cat_[CatIndex.MISTAKE_DEFENCE_] = true;
        is_unlock_cat_[CatIndex.BONUS_STAGE_] = true;

        level_cat_[CatIndex.BONUS_STAGE_] = 2;
        level_cat_[CatIndex.MISTAKE_DEFENCE_] = 2;
        level_cat_[CatIndex.TOTAL_TIME_UP_] = 2;

        selected_cat_[0] = CatIndex.BONUS_STAGE_;
        selected_cat_[1] = CatIndex.MISTAKE_DEFENCE_;
        selected_cat_[2] = CatIndex.TOTAL_TIME_UP_;
        //!!!!임시코드 고양이 지정
    }

}
