using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Networking;

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



    private string email_;
    private string password_;
    private string username_;

    private string nickname_ = "Player";
    private int coin_=0;
    private float exp_ = 0.0f;

    private int[] selected_cat_ = { -1,-1,-1};
    private bool[] is_unlock_cat_;
    private int[] level_cat_;
    private float[] exp_cat_;





    //===========서버 통신=============

    private AuthUIManager authManager;

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

    public void SendSignUpRequest(AuthUIManager auth, string email, string nickname, string username, string password) {
        authManager = auth;
        StartCoroutine(SignUpRequest(email, nickname, username, password));
    }

    public void SendLoginRequest(AuthUIManager auth, string username, string password)
    {
        authManager = auth;
        StartCoroutine(LoginRequest(username,password));
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

        UnityWebRequest web_request = new UnityWebRequest("http://3.237.76.145:8080/member/sign", "POST");
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
                if (authManager != null) 
                {
                    authManager.Unlock();
                    authManager.ShowToast($"회원가입 성공! 로그인 하세요!");
                    authManager.ShowLogin();
                }
            }
            catch
            {
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast("응답 파싱 오류 (성공) parsing error(success)");
                }
            }
        }
        else
        {
            try
            {
                LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(web_request.downloadHandler.text);
                if (authManager != null)
                {
                    Debug.Log("회원가입에 실패함 : " + error.message.ToString() + " / "+ error.description.ToString());
                    authManager.Unlock();
                    authManager.ShowToast($"회원가입 실패: {error.message}\n{error.description}");
                }
            }
            catch
            {
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast("응답 파싱 오류 (실패) parsing error(failure)");
                }
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

        UnityWebRequest web_request = new UnityWebRequest("http://3.237.76.145:8080/member/login", "POST");
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
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast($"로그인 성공! 로그인 하세요!");
                    authManager.CloseUI();
                }
            }
            catch
            {
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast("응답 파싱 오류 (성공)");
                }
            }
        }
        else
        {
            try
            {
                LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(web_request.downloadHandler.text);
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast($"Login Faliure: {error.message}\n{error.description}");
                }
            }
            catch
            {
                if (authManager != null)
                {
                    authManager.Unlock();
                    authManager.ShowToast("응답 파싱 오류 (실패)");
                }
            }
        }
    }






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
        is_unlock_cat_[CatIndex.LIFE_REMOVE_DOWN_] = true;
        is_unlock_cat_[CatIndex.TILE_SPEED_DOWN_] = true;
        is_unlock_cat_[CatIndex.FEVER_UP_] = true;
        is_unlock_cat_[CatIndex.TIME_STOP_] = true;
        is_unlock_cat_[CatIndex.SIMPLE_LINE_] = true;
        is_unlock_cat_[CatIndex.BONUS_STAGE_] = true;
        is_unlock_cat_[CatIndex.SAVOTAGE_DEFENCE_] = true;

        level_cat_[CatIndex.BONUS_STAGE_] = 5;
        level_cat_[CatIndex.MISTAKE_DEFENCE_] = 2;
        level_cat_[CatIndex.TOTAL_TIME_UP_] = 2;
        level_cat_[CatIndex.ROUND_TIME_UP_] = 2;
        level_cat_[CatIndex.LIFE_REMOVE_DOWN_] = 2;
        level_cat_[CatIndex.TILE_SPEED_DOWN_] = 2;
        level_cat_[CatIndex.FEVER_UP_] = 2;
        level_cat_[CatIndex.TIME_STOP_] = 2;
        level_cat_[CatIndex.SIMPLE_LINE_] = 5;
        level_cat_[CatIndex.SAVOTAGE_DEFENCE_] = 2;

        selected_cat_[0] = CatIndex.SIMPLE_LINE_;
        selected_cat_[1] = CatIndex.MISTAKE_DEFENCE_;
        selected_cat_[2] = CatIndex.BONUS_STAGE_;
        //!!!!임시코드 고양이 지정
    }

}
