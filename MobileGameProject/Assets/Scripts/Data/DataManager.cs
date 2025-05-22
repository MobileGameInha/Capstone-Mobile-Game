using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;


public enum Item { SNACK, BELL, BOX, DISK, TICKET, FLOWER, LEAF, EARTH }

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

    public delegate void RequestSuccessDelegate();
    public delegate void RequestFailDelegate(string err);

    public const float PLAYER_PER_EXP = 500.0f;
    public const float MAX_CAT_EXP = 100.0f;

    private const string SERVER_API_BASIC_ADDRESS = "http://44.195.91.215:8080";

    private bool requesting_ = false;

    public RequestSuccessDelegate requestSuccededDelegate;
    public RequestFailDelegate requestFailedDelegate;

    private string email_;
    private string password_;
    private string username_;

    private int profile_image_=0;
    private string nickname_ = "Player";
    private int coin_=0;
    private float exp_ = 0.0f;

    private bool[] isUnlockStage = { true, false, false, false, false };
    private bool isUnlockChallange = false;

    private int[] selected_cat_ = { -1,-1,-1};
    private bool[] is_unlock_cat_ = new bool[GameManager.CAT_SIZE_];
    private int[] level_cat_ = new int[GameManager.CAT_SIZE_];
    private float[] exp_cat_ = new float[GameManager.CAT_SIZE_];

    private int[] inventory = { 0,0,0,0,0,0,0,0 };

    

    //===========서버 통신=============

    public bool GetIsRequesting() { return requesting_; }

    [System.Serializable]
    public class SignUpData
    {
        public string username;
        public string email;
        public string nickname;
        public string password;
    }

    [System.Serializable]
    public class LoginSuccessResponse
    {
        public int id;
    }

    [System.Serializable]
    public class LoginErrorResponse
    {
        public string message;
        public string description;
    }
    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }

    public void SendSignUpRequest(string email, string nickname, string username, string password) {
        requesting_ = true;
        StartCoroutine(SignUpRequest(email, nickname, username, password));
    }

    public void SendLoginRequest(string username, string password)
    {
        requesting_ = true;
        StartCoroutine(LoginRequest(username,password));
    }

    public void GetPlayerData()
    { 
        //+)플레이어 데이터 끌어오기 ~ 로그인에 합쳐도 됨
    }

    public void SetPlayerProfile(int idx)
    {
        //+)플레이어 프로필 이미지 변경
    }

    public void RemoveCoin(int value)
    {
        //+)플레이어 코인 제거
    }

    public void UnlockCat(int idx)
    {
        //+)고양이 해금
    }

    public void UpgradeCat(int idx)
    {
        //+)고양이 업그레이드
    }

    public void GetItem(int idx)
    {
        //+)아이템 얻기
    }

    public void GetRankingData()
    {
        //+)랭킹데이터 끌어오기
    }

    public void ChangeDataOfEndGame(int score)
    {
        //+)게임 종료 후 데이터 전송
    }

    private IEnumerator SignUpRequest(string email, string nickname, string username, string password)
    {
        Debug.Log("서버에 회원가입 데이터를 SEND합니다.");

        SignUpData requestData = new SignUpData
        {
            username = username,
            email = email,
            nickname = nickname,
            password = password
        };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS+"/member/sign", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        Debug.Log("서버에서 회원가입 데이터를 받아왔습니다.");

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
                requestSuccededDelegate();
            }
            catch
            {
                requestFailedDelegate("오류가 발생했습니다. 다시 시도해주세요");
            }
        }
        else
        {
            try
            {
                LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(web_request.downloadHandler.text);
                requestFailedDelegate("회원가입에 실패했습니다. 다시 시도해주세요");
            }
            catch
            {
                requestFailedDelegate("오류가 발생했습니다. 다시 시도해주세요");
            }
        }

    }

    private IEnumerator LoginRequest(string username, string password)
    {
        LoginData loginData = new LoginData
        {
            username = username,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/member/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
                requestSuccededDelegate();
            }
            catch
            {
                requestFailedDelegate("오류가 발생했습니다. 다시 시도해주세요");
            }
        }
        else
        {
            try
            {
                LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(web_request.downloadHandler.text);
                requestFailedDelegate("로그인에 실패했습니다. 다시 시도해주세요");
            }
            catch
            {
                requestFailedDelegate("오류가 발생했습니다. 다시 시도해주세요");
            }
        }
    }


    //======================데이터 참조================================
    public int GetProfileImage() { return profile_image_; }
    public string GetNickName() { return nickname_; }
    public int GetCoin() { return coin_; }
    public int GetLevel() { return Mathf.FloorToInt(exp_ / PLAYER_PER_EXP); }
    public float GetRemainEXP() { return exp_ - GetLevel()*PLAYER_PER_EXP; }

    public bool GetIsUnlockStage(int idx) { return isUnlockStage[idx]; }
    public bool GetIsUnlockChallangeStage() { return isUnlockChallange; }

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

        if (idx>=0 && idx<=2 && selected_cat_[idx]!=-1 && is_unlock_cat_[selected_cat_[idx]])
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

    public float GetEXPOfCat(int idx)
    {
        return exp_cat_[idx];
    }

    public int GetItemCount(Item item)
    {
        return inventory[(int)item];
    }

    public void RequestRemoveCoin(int remove_value) 
    {
        coin_ -= remove_value;//+)임시 : 
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
            level_cat_[i] = 1;
            exp_cat_[i] = 0.0f;
        }






        //!!!!임시코드
        inventory = new int[]{ 0,1,2,3,4,5,6,7 };
        coin_ = 10000;
        profile_image_ = 2;

        isUnlockStage[1] = true;

        is_unlock_cat_[CatIndex.TOTAL_TIME_UP_] = true;
        is_unlock_cat_[CatIndex.ROUND_TIME_UP_] = true;
        is_unlock_cat_[CatIndex.MISTAKE_DEFENCE_] = true;
        is_unlock_cat_[CatIndex.LIFE_REMOVE_DOWN_] = true;
        is_unlock_cat_[CatIndex.TILE_SPEED_DOWN_] = true;
        is_unlock_cat_[CatIndex.FEVER_UP_] = true;
        is_unlock_cat_[CatIndex.SIMPLE_LINE_] = true;
        is_unlock_cat_[CatIndex.BONUS_STAGE_] = true;
        is_unlock_cat_[CatIndex.SAVOTAGE_DEFENCE_] = true;

        exp_cat_[CatIndex.TOTAL_TIME_UP_] = 50.0f;
        exp_cat_[CatIndex.ROUND_TIME_UP_] = 50.0f;

        level_cat_[CatIndex.BONUS_STAGE_] = 5;
        level_cat_[CatIndex.MISTAKE_DEFENCE_] = 2;
        level_cat_[CatIndex.TOTAL_TIME_UP_] = 2;
        level_cat_[CatIndex.ROUND_TIME_UP_] = 2;
        level_cat_[CatIndex.LIFE_REMOVE_DOWN_] = 2;
        level_cat_[CatIndex.TILE_SPEED_DOWN_] = 2;
        level_cat_[CatIndex.FEVER_UP_] = 2;
        level_cat_[CatIndex.SIMPLE_LINE_] = 5;
        level_cat_[CatIndex.SAVOTAGE_DEFENCE_] = 2;

        selected_cat_[0] = CatIndex.SIMPLE_LINE_;
        selected_cat_[1] = CatIndex.MISTAKE_DEFENCE_;
        selected_cat_[2] = CatIndex.BONUS_STAGE_;
        //!!!!임시코드
    }

}
