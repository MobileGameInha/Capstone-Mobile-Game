using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using static UnityEngine.Rendering.DebugUI;

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
    }//DataManager를 싱글톤으로 설정

    private static DataManager dataManager_instance; //싱글톤 인스턴스

    public delegate void RequestSuccessDelegate();
    public delegate void RequestFailDelegate(string err);

    public const float PLAYER_PER_EXP = 100.0f;
    public const float MAX_CAT_EXP = 100.0f;

    private const string SERVER_API_BASIC_ADDRESS = "http://3.231.148.170:8080";

    private readonly int[] STAGE_UNLOCK_LEVEL = {0,3,6,10,13};
    private readonly int STAGE_UNLOCK_CHALLENGE = 15;

 
    public static readonly int COIN_PER_SCORE = 75;
    public static readonly int EXP_PER_SCORE = 125;

    public static readonly int ITEM_PRICE = 200;

    private bool requesting_ = false;

    public RequestSuccessDelegate requestSuccededDelegate;
    public RequestFailDelegate requestFailedDelegate;

    public RequestSuccessDelegate requestSuccededDelegateForRank;
    public RequestFailDelegate requestFailedDelegateForRank;

    private int inherence_id_; // 유저 아이디

    private string email_;
    private string password_;
    private string username_;

    [SerializeField]
    private int profile_image_=0;
    [SerializeField]
    private string nickname_ = "Player";
    [SerializeField]
    private int coin_=0;
    [SerializeField]
    private float exp_ = 0.0f;

    [SerializeField]
    private bool[] isUnlockStage = { true, false, false, false, false };
    [SerializeField]
    private bool isUnlockChallange = false;

    [SerializeField]
    private int[] selected_cat_ = { -1,-1,-1};
    [SerializeField]
    private bool[] is_unlock_cat_ = new bool[GameManager.CAT_SIZE_];
    [SerializeField]
    private int[] level_cat_ = new int[GameManager.CAT_SIZE_];
    [SerializeField]
    private float[] exp_cat_ = new float[GameManager.CAT_SIZE_];

    [SerializeField]
    private int[] inventory = { 0,0,0,0,0,0,0,0 };


    [SerializeField]
    private int player_total_score_ = 0;
    [SerializeField]
    private int[] player_max_score_ = { 0,0,0,0,0,0,0,0};
    [SerializeField]
    private int[] player_rank_ = { 0, 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField]
    private int player_tier_ = 0;

    [SerializeField]
    private int[,] profile_of_rankers_ = {
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 }
    };

    [SerializeField]
    private int[,] level_of_rankers_ = {
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 }
    };

    [SerializeField]
    private int[,] score_of_rankers_ = {
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 },
    { 0,0,0 }
    };

    [SerializeField]
    private string[,] nickname_of_rankers_ = {
    { "","","" },
    { "","","" },
    { "","","" },
    { "","","" },
    { "","","" },
    { "","","" },
    { "","","" },
    { "","","" }
    };

    //===========���� ���=============

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
    public class ErrorResponse
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

    [System.Serializable]
    public class UserBasicData
    {
        public int id;
        public string nickname;
        public string username;
        public float totalExp;
        public int gold;
        public int profileNumber;
    }

    [System.Serializable]
    public class CatData
    {
        public int helperId;
        public float exp;
        public int level;
        public bool active;
    }

    [System.Serializable]
    public class UserCatData
    {
        public CatData[] catHelpers;
    }

    [System.Serializable]
    public class UserProfileImageData
    {
        public int profileNumber;
    }
    //프로필 데이터

    [System.Serializable]
    public class CatPriceData
    {
        public int helperPrice;
    }
    //고양이 구매 데이터

    [System.Serializable]
    public class CatGetSelectData
    {
        public int[] catHelperIds;
    }
    //고양이 선택 데이터 끌어오기

    [System.Serializable]
    public class CatSelectData
    {
        public int[] helperIds;
    }
    //고양이 선택 데이터



    [System.Serializable]
    public class CatUpgradeData
    {
        public int[] items;
        public int itemCount;
    }
    //고양이 업그레이드 데이터

    [System.Serializable]
    public class Score
    {
        public int score;
    }
    //점수 데이터
    [System.Serializable]
    public class ScoreData
    {
        public int exp;
        public int memberGold;
        public int[] helperIds;
    }
    //점수에 따른 갱신 데이터

    [System.Serializable]
    public class ItemPriceData
    {
        public int itemNumber;
        public int memberId;
    }
    //아이템 데이터
    [System.Serializable]
    public class ItemData
    {
        public int itemNumber;
        public int count;
    }
    //아이템 획득데이터
    [System.Serializable]
    public class ItemDataAll
    {
        public ItemData[] inventories;
    }
    //아이템 전체 데이터

    [System.Serializable]
    public class topRanks
    {
        public int memberId;
        public string memberName;
        public int score;
        public int rank;
        public int profileNumber;
        public int totalExp;
    }

    [System.Serializable]
    public class myRank
    {
        public int memberId;
        public string memberName;
        public int score;
        public int rank;
        public int profileNumber;
        public int totalExp;
    }

    [System.Serializable]
    public class RankData
    {
        public topRanks[] topRanks;
        public myRank myRank;
    }
    //랭크 데이터

    [System.Serializable]
    public class TierData
    {
        public string tier;
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

    private void GetPlayerData()
    {
        StartCoroutine(GetUserDataRequest());
    }

    public void SetPlayerProfile(int idx)
    {
        if (idx >= 0 && idx < 9)
        {
            requesting_ = true;
            profile_image_ = idx;
            StartCoroutine(ChangeProfileIndexRequest(idx));
        }
        else {
            if (requestFailedDelegate != null)
                requestFailedDelegate("프로필변경에 실패하였습니다!");
        }
    }//�߿����� ���� send�̱⿡ requesting_ = true; ���� ����

    public void SetSelectedCat(int which_cat, int cat_idx)
    {
        if (which_cat < 0 || which_cat >= BasicHelperManager.MAX_HELPER_ ||  cat_idx >= GameManager.CAT_SIZE_)
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("고양이 선택에 실패하였습니다!");
        }

        requesting_ = true;

        int[] helperIndex = new int[BasicHelperManager.MAX_HELPER_];


        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            if (i == which_cat)
            {
                helperIndex[i] = cat_idx +1;
            }
            else
            {
                if (selected_cat_[i] == cat_idx)
                {
                    helperIndex[i] = 0;
                }
                else
                {
                    helperIndex[i] = selected_cat_[i] + 1;
                }
            }
        }

        StartCoroutine(SelectCatRequest(helperIndex));
    }

    public void UnlockCat(int idx)
    {
        if (idx < 0 || idx >= GameManager.CAT_SIZE_)
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("잘못된 접근입니다.");
            return;
        }

        if (coin_ < BasicShopManager.CAT_COST_LIST[idx])
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("비용이 부족합니다.");
            return;
        }

        if (is_unlock_cat_[idx])
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("이미 해금한 고양이 입니다.");
            return;
        }

        requesting_ = true;

        StartCoroutine(BuyCatRequest(BasicShopManager.CAT_COST_LIST[idx], idx));

    }

    public void UpgradeCat(int idx)
    {
        if (idx < 0 || idx >= GameManager.CAT_SIZE_)
        {
            requestFailedDelegate("잘못된 접근입니다.");
        }

        if (exp_cat_[idx] >= 100.0f && level_cat_[idx] < 4 &&
            BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[idx]] <= inventory[BasicHelperManager.CAT_UPGRADE_LIST_[idx, 0]]
            && BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[idx]] <= inventory[BasicHelperManager.CAT_UPGRADE_LIST_[idx, 1]]
            && BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[idx]] <= inventory[BasicHelperManager.CAT_UPGRADE_LIST_[idx, 2]]
            )
        {

            requesting_ = true;
            StartCoroutine(UpgradeCatRequest(idx));

        }
        else 
        {
            requestFailedDelegate("잘못된 접근입니다.");
        }
    }

    public void GetItem(int idx)
    {
        requesting_ = true;
        StartCoroutine(AddItemRequest(idx));
    }

    public void GetRankingData()
    {
        StartCoroutine(GetRankDataRequest());
    }

    public void UpdateScore(int score, int stage_id)
    {
        requesting_ = true;
        StartCoroutine(UpdateScoreRequest(score, stage_id));
    }

    private IEnumerator SignUpRequest(string email, string nickname, string username, string password)
    {

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


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
                if(requestSuccededDelegate!=null)
                    requestSuccededDelegate();
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("회원가입에 실패하였습니다.\n다시 시도해주세요.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("회원가입에 실패하였습니다.\n아이디와 비밀번호를 변경하여\n다시 시도해주세요.");
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("회원가입에 실패하였습니다.\n다시 시도해주세요.");
            }
        }

        requesting_ = false;
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


        bool is_succeded = false;

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
                inherence_id_ = success.id;
                is_succeded = true;
                //requestSuccededDelegate();
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("로그인에 실패하였습니다.\n다시 시도해주세요.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("로그인에 실패하였습니다.\n다시 시도해주세요.");
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("로그인에 실패하였습니다.\n다시 시도해주세요.");
            }
        }
        if (is_succeded) {
            GetPlayerData();
        }
        else
        {
            requesting_ = false;
        }
    }

    private IEnumerator GetUserDataRequest()
    {
        bool is_success = true;


        ErrorResponse error = new ErrorResponse();
        error.message = "데이터를 불러오는데 실패하였습니다.";

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/member/info/"+inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UserBasicData response = JsonUtility.FromJson<UserBasicData>(web_request.downloadHandler.text);
                nickname_ = response.nickname;
                username_ = response.username;
                coin_ = response.gold;
                exp_ = response.totalExp;
                profile_image_ = response.profileNumber;

                SetStageUnlock();
                Debug.Log("유저데이터 가져오기 성공");
            }
            catch
            {
                Debug.Log("유저데이터 가져오기 중도 실패");
                is_success = false;
            }
        }
        else
        {
            is_success = false;

            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                Debug.Log("데이터 불러오기 실패");
            }
        }



        web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/all/" + inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UserCatData response = JsonUtility.FromJson<UserCatData>(web_request.downloadHandler.text);
                for (int i = 0; i < response.catHelpers.Length; i++)
                {
                    if (response.catHelpers[i] != null && response.catHelpers[i].helperId>=1 && response.catHelpers[i].helperId<=GameManager.CAT_SIZE_)
                    {
                        is_unlock_cat_[response.catHelpers[i].helperId-1] = true;
                        exp_cat_[response.catHelpers[i].helperId - 1] = response.catHelpers[i].exp;
                        level_cat_[response.catHelpers[i].helperId - 1] = response.catHelpers[i].level;
                    }
                }
                Debug.Log("고양이 정보 가져오기 성공");
            }
            catch
            {
                Debug.Log("고양이 정보 가져오는 중 실패");
                is_success = false;
            }
        }
        else
        {
            is_success = false;

            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch
            {
            }
        }

        web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/choose/" + inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                CatGetSelectData response = JsonUtility.FromJson<CatGetSelectData>(web_request.downloadHandler.text);
                Debug.Log("IDS Length" + response.catHelperIds.Length);
                for (int i = 0; i < response.catHelperIds.Length; i++)
                {
                    Debug.Log(i+"번째" + (response.catHelperIds[i] - 1).ToString());
                    selected_cat_[i] = response.catHelperIds[i]-1;
                }
                Debug.Log("선택된 고양이 정보 가져오는 중");
            }
            catch
            {
                Debug.Log("선택된 고양이 정보 가져오다 실패");
                is_success = false;
            }
        }
        else
        {
            is_success = false;

            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch
            {
            }
        }

        web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/inventory/" + inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                ItemDataAll response = JsonUtility.FromJson<ItemDataAll>(web_request.downloadHandler.text);

                if (response.inventories!= null)
                {

                    for (int i = 0; i < response.inventories.Length; i++)
                    {
                        inventory[response.inventories[i].itemNumber - 1] = response.inventories[i].count;
                    }
                    Debug.Log("아이템 정보 가져오는 중");
                }
            }
            catch
            {
                Debug.Log("아이템 정보 가져오다 실패");
                is_success = false;
            }
        }
        else
        {
            is_success = false;

            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch
            {
            }
        }



        if (is_success)
        {
            if (requestSuccededDelegate != null)
                requestSuccededDelegate();
        }
        else {
            if (requestFailedDelegate != null)
                requestFailedDelegate(error.message);
        }

        requesting_ = false;
    }

    private IEnumerator ChangeProfileIndexRequest(int profileNumber)
    {

        UserProfileImageData ProfileImageData = new UserProfileImageData
        {
            profileNumber = profileNumber
        };

        string jsonData = JsonUtility.ToJson(ProfileImageData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/member/number/update/" + inherence_id_.ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();

                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if(requestFailedDelegate!=null)
                    requestFailedDelegate("프로필 이미지 변경에 실패하였습니다.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("프로필 이미지 변경에 실패하였습니다.\n"+ error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("프로필 이미지 변경에 실패하였습니다.");
            }
        }
    }

    private IEnumerator BuyCatRequest(int helperPrice, int cat_idx)
    {

        CatPriceData requestData = new CatPriceData
        {
            helperPrice = helperPrice
        };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/buy/" + inherence_id_.ToString() + "/" + (cat_idx + 1).ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                coin_ -= helperPrice;
                is_unlock_cat_[cat_idx] = true;

                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();

                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 구매에 실패하였습니다.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 구매에 실패하였습니다.\n" + error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 구매에 실패하였습니다.\n");
            }
        }

        requesting_ = false;
    }

    private IEnumerator SelectCatRequest(int[] helperIds)
    {

        CatSelectData requestData = new CatSelectData
        {
            helperIds = helperIds
        };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/choose/" + inherence_id_.ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                for (int i = 0; i < helperIds.Length; i++)
                {
                    selected_cat_[i] = helperIds[i] - 1;
                }

                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();

                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 선택에 실패하였습니다.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 선택에 실패하였습니다.\n" + error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 선택에 실패하였습니다.");
            }
        }

        requesting_ = false;
    }

    private IEnumerator UpdateScoreRequest(int score, int stage_id) 
    {
        bool is_success = true;

        ErrorResponse error = new ErrorResponse();

        int[] tmp_cats = new int[BasicHelperManager.MAX_HELPER_];
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            tmp_cats[i] = selected_cat_[i] + 1;
        }

        ScoreData requestData = new ScoreData
        {
            exp = score / EXP_PER_SCORE,
            memberGold = score / COIN_PER_SCORE,
            helperIds = tmp_cats
        };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/stage/" + inherence_id_.ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("스코어에 따른 갱신 성공");
            try
            {
                coin_ += score / COIN_PER_SCORE;
                exp_ += score / EXP_PER_SCORE;
                for (int i = 0; i < selected_cat_.Length; i++)
                {
                    if (selected_cat_[i] >= 0 && selected_cat_[i] < GameManager.CAT_SIZE_ && level_cat_[i]<4)
                    {
                        exp_cat_[selected_cat_[i]] += score / EXP_PER_SCORE;
                        if (exp_cat_[selected_cat_[i]] >= 100.0f) { exp_cat_[selected_cat_[i]] = 100.0f; }
                    }
                }

                SetStageUnlock();

                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                Debug.Log("스코어에 따른 갱신 도중 실패");
                is_success = false;
            }
        }
        else
        {
            Debug.Log("스코어에 따른 갱신 실패");
            is_success = false;
            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch { }
        }

        Score requestData2 = new Score
        {
            score = score
        };

        jsonData = JsonUtility.ToJson(requestData2);

        //+)TODO Fill next Sentence
        web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/stage/" + inherence_id_.ToString() + "/" + (stage_id+1).ToString(), "POST");
        bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("스코어 갱신 성공");
            try
            {
                if (player_max_score_[stage_id] < score) {
                    player_max_score_[stage_id] = score;
                }
            }
            catch
            {
                Debug.Log("스코어 갱신 도중 실패");
                is_success = false;
            }
        }
        else
        {
            Debug.Log("스코어 갱신 실패");
            is_success = false;

            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
            }
            catch { }
        }

        if (is_success)
        {
            if (requestSuccededDelegate != null)
                requestSuccededDelegate();
        }
        else
        {
            if (requestFailedDelegate != null)
                if (error != null)
                {
                    requestFailedDelegate(error.message);
                }
                else {
                    requestFailedDelegate("예기치 못한 오류가 발생하였습니다!");
                }
        }

        requesting_ = false;
    }


    private IEnumerator UpgradeCatRequest(int cat_idx)
    {

        int[] removeitems = new int[3];
        removeitems[0] = BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 0] + 1;
        removeitems[1] = BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 1] + 1;
        removeitems[2] = BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 2] + 1;

        CatUpgradeData requestData = new CatUpgradeData
        {
            items = removeitems,
            itemCount = BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[cat_idx]]
        };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/detail/" + inherence_id_.ToString() +"/"+ (cat_idx+1).ToString(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                inventory[BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 0]] -= BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[cat_idx]];
                inventory[BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 1]] -= BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[cat_idx]];
                inventory[BasicHelperManager.CAT_UPGRADE_LIST_[cat_idx, 2]] -= BasicHelperManager.CAT_UPGRATE_COUNT_[level_cat_[cat_idx]];

                level_cat_[cat_idx]++;
                exp_cat_[cat_idx] = 0.0f;

                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();
                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 업그레이드에 실패하였습니다.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 업그레이드에 실패하였습니다.\n" + error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("고양이 업그레이드에 실패하였습니다.");
            }
        }

        requesting_ = false;
    }

    private IEnumerator AddItemRequest(int item_idx)
    {

        ItemPriceData requestData = new ItemPriceData
        {
            itemNumber = item_idx + 1,
            memberId = inherence_id_
        };

        string jsonData = JsonUtility.ToJson(requestData);

        //+)TODO Fill next Sentence
        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/inventory", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        web_request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        web_request.downloadHandler = new DownloadHandlerBuffer();
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                coin_ -= ITEM_PRICE;
                inventory[item_idx]++;

                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();
                //LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("아이템 획득에 실패하였습니다.");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("아이템 획득에 실패하였습니다.\n" + error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("아이템 획득에 실패하였습니다.");
            }
        }

        requesting_ = false;
    }

    private IEnumerator GetRankDataRequest()
    {
        bool is_success = true;
        ErrorResponse error = new ErrorResponse();

        player_total_score_ = 0;

        for (int i = 0; i < 8; i++)
        {
            Debug.Log(i.ToString() + "번째 스테이지 데이터를 불러옵니다");
            UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/stage/"+(i + 1).ToString() +"/"+inherence_id_.ToString()+"/ranking", "GET");
            web_request.downloadHandler = new DownloadHandlerBuffer();
            web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return web_request.SendWebRequest();

            if (web_request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    Debug.Log(i.ToString() + "번째 스테이지 데이터를 불러오기 시작합니다.");
                    LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
                    RankData response = JsonUtility.FromJson<RankData>(web_request.downloadHandler.text);
                    for (int j = 0; j < response.topRanks.Length; j++)
                    {
                        level_of_rankers_[i, j] = (int)(Mathf.RoundToInt(response.topRanks[j].totalExp) / PLAYER_PER_EXP);
                        nickname_of_rankers_[i, j] = response.topRanks[j].memberName;
                        profile_of_rankers_[i, j] = response.topRanks[j].profileNumber;
                        score_of_rankers_[i,j] = response.topRanks[j].score;
                    }

                    player_total_score_ += response.myRank.score;
                    player_max_score_[i] =  response.myRank.score;
                    player_rank_[i] = response.myRank.rank;
                    Debug.Log(i.ToString() + "번째 스테이지 데이터를 잘 불러왔습니다.");
                }
                catch
                {
                        is_success = false;
                    Debug.Log(i.ToString() + "번째 스테이지 데이터를 불러오다 실패했습니다.");
                }
            }
            else
            {
                Debug.Log(i.ToString() + "번째 스테이지 데이터를 불러오지 못했습니다.");
                is_success = false;
                try
                {
                    error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                    Debug.Log(i.ToString() + "오류 : "+ error.message + error.description);
                }
                catch
                {
                }
            }

         }

        Debug.Log("유저 티어 데이터를 불러오기 시작합니다.");
        UnityWebRequest web_request_two = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/stage/tier/"+inherence_id_.ToString(), "GET");
        web_request_two.downloadHandler = new DownloadHandlerBuffer();
        web_request_two.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return web_request_two.SendWebRequest();

        if (web_request_two.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request_two.downloadHandler.text);
                TierData response = JsonUtility.FromJson<TierData>(web_request_two.downloadHandler.text);

                if (response.tier == "Bronze")
                {
                    Debug.Log("브론즈입니다");
                    player_tier_ = 0;
                }
                else if (response.tier == "Gold")
                {
                    Debug.Log("골드입니다");
                    player_tier_ = 1;
                }
                else if (response.tier == "Diamond")
                {
                    Debug.Log("다이아입니다");
                    player_tier_ = 2;
                }
                else if (response.tier == "Master")
                {
                    Debug.Log("마스터입니다");
                    player_tier_ = 3;
                }
                else if (response.tier == "Challenger")
                {
                    Debug.Log("챌린저입니다");
                    player_tier_ = 4;
                }
                else {
                    Debug.Log("받아오지 못했습니다 : 브론즈");
                    player_tier_ = 0;
                }
                Debug.Log("유저 티어 데이터를 잘 불러왔습니다.");
            }
            catch
            {
                Debug.Log("유저 티어 데이터를 불러오다 실패했습니다.");
                is_success = false;
            }
        }
        else
        {
            Debug.Log("유저 티어 데이터를 불러오지 못했습니다.");
            is_success = false;
            try
            {
                error = JsonUtility.FromJson<ErrorResponse>(web_request_two.downloadHandler.text);
                Debug.Log( "티어 오류 : " + error.message + error.description);
            }
            catch
            {
            }
        }

        if (is_success)
        {
            if (requestSuccededDelegateForRank != null)
                requestSuccededDelegateForRank();
        }
        else
        {
            if (requestFailedDelegateForRank != null)
                requestFailedDelegateForRank(error.message);
        }

    }


    //======================데이터 변경 로직================================
    private void SetStageUnlock() {
        int level = GetLevel();
        for (int i = 0; i < isUnlockStage.Length; i++)
        {
            if (level >= STAGE_UNLOCK_LEVEL[i])
            {
                isUnlockStage[i] = true;
            }
            else
            {
                isUnlockStage[i] = false;
            }
        }

        if (level >= STAGE_UNLOCK_CHALLENGE)
        {
            isUnlockChallange = true;
        }
        else
        {
            isUnlockChallange = false;
        }

    }


    //======================데이터 획득 로직================================
    public int GetProfileImage() { return profile_image_; }
    public string GetNickName() { return nickname_; }
    public int GetCoin() { return coin_; }
    public int GetLevel() { return Mathf.FloorToInt(exp_ / PLAYER_PER_EXP); }
    public float GetRemainEXP() { return exp_ - GetLevel()*PLAYER_PER_EXP; }

    public bool GetIsUnlockStage(int idx) { return isUnlockStage[idx]; }
    public bool GetIsUnlockChallangeStage() { return isUnlockChallange; }



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
        coin_ -= remove_value;
    }//아직 사용처 없음

    public int GetPlayerTotalScore() { return player_total_score_; }

    public int GetPlayerMaxScore(int stage) { return player_max_score_[stage]; } 

    public int GetPlayerRank(int stage) { return player_rank_[stage]; }

    public int GetPlayerTier() { return player_tier_; }

    public int GetProfileOfRanker(int stage, int idx) { return profile_of_rankers_[stage, idx]; }
    public int GetLevelOfRanker(int stage, int idx) { return level_of_rankers_[stage, idx]; }

    public int GetScoreOfRanker(int stage, int idx) { return score_of_rankers_[stage, idx]; }
    public string GetNicknameOfRanker(int stage, int idx) { return nickname_of_rankers_[stage, idx]; }

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
            exp_cat_[i] = 0.0f;
        }
    }

}
