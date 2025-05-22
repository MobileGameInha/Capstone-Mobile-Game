using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    }//DataManager�� �̱������� ����

    private static DataManager dataManager_instance; //�̱��� �ν��Ͻ�

    public delegate void RequestSuccessDelegate();
    public delegate void RequestFailDelegate(string err);

    public const float PLAYER_PER_EXP = 100.0f;
    public const float MAX_CAT_EXP = 100.0f;

    private const string SERVER_API_BASIC_ADDRESS = "http://18.204.43.221:8080";

    private readonly int[] STAGE_UNLOCK_LEVEL = {0,3,6,10,13};
    private readonly int STAGE_UNLOCK_CHALLENGE = 15;

    private bool requesting_ = false;

    public RequestSuccessDelegate requestSuccededDelegate;
    public RequestFailDelegate requestFailedDelegate;

    private int inherence_id_; // �÷��̾� �ĺ� ���� ���̵�

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
    //���� ������ �̹��� ����

    [System.Serializable]
    public class CatPriceData
    {
        public int helperPrice;
    }
    //������ ����� ����

    [System.Serializable]
    public class CatSelectData
    {
        public int[] helperIds;
    }
    //������ ����� ����




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
                requestFailedDelegate("�ε��� ����");
        }
    }

    public void SetHelper(int which_cat, int cat_idx)
    {
        if (which_cat < 0 || which_cat >= BasicHelperManager.MAX_HELPER_ || cat_idx < 0 || cat_idx >= GameManager.CAT_SIZE_)
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("�ε��� ����");
        }

        requesting_ = true;

        int[] helperIndex = new int[BasicHelperManager.MAX_HELPER_];
        for (int i = 0; i < BasicHelperManager.MAX_HELPER_; i++)
        {
            if (i == which_cat)
            {
                helperIndex[i] = cat_idx;
            }
            else
            {
                helperIndex[i] = selected_cat_[i];
            }
        }
    }

    public void UnlockCat(int idx)
    {
        if (idx < 0 || idx >= GameManager.CAT_SIZE_)
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("�ε��� ����");
            return;
        }

        if (coin_ < BasicShopManager.CAT_COST_LIST[idx])
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("�ݾ� ����");
            return;
        }

        if (is_unlock_cat_[idx])
        {
            if (requestFailedDelegate != null)
                requestFailedDelegate("�̹� ������ �����");
            return;
        }

        StartCoroutine(BuyCatRequest(BasicShopManager.CAT_COST_LIST[idx], idx));

    }

    public void UpgradeCat(int idx)
    {
        //+)����� ���׷��̵�
    }

    public void GetItem(int idx)
    {
        //+)������ ���
    }

    public void GetRankingData()
    {
        //+)��ŷ������ �������
    }

    public void ChangeDataOfEndGame(int score)
    {
        //+)���� ���� �� ������ ����
    }

    private IEnumerator SignUpRequest(string email, string nickname, string username, string password)
    {
        Debug.Log("������ ȸ������ �����͸� SEND�մϴ�.");

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

        Debug.Log("�������� ȸ������ �����͸� �޾ƿԽ��ϴ�.");

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
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("ȸ�����Կ� �����߽��ϴ�. �ٽ� �õ����ּ���");
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }

        requesting_ = false;
    }

    private IEnumerator LoginRequest(string username, string password)
    {
        Debug.Log("������ �α��� �����͸� SEND�մϴ�.");

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

        Debug.Log("�������� �α��� �����͸� �޾ƿԽ��ϴ�.");

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
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("�α��ο� �����߽��ϴ�. �ٽ� �õ����ּ���");
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
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
        Debug.Log("������ �����͸� �޾ƿɴϴ�.");
        bool is_success = true;

        //���� �⺻ ������

        ErrorResponse error = new ErrorResponse();
        error.message = "����ġ ���� ������ �߻��Ͽ����ϴ�.";

        UnityWebRequest web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/member/info/"+inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();


        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.Log("������ �޾ƿ���...");
                UserBasicData response = JsonUtility.FromJson<UserBasicData>(web_request.downloadHandler.text);
                nickname_ = response.nickname;
                username_ = response.username;
                coin_ = response.gold;
                exp_ = response.totalExp;
                profile_image_ = response.profileNumber;

                SetStageUnlock();

            }
            catch
            {
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
                Debug.Log("���� �������⵵ ����");
            }
        }


        //���� ������ ������


        Debug.Log("������ ������ �����͸� �޾ƿɴϴ�");
        web_request = new UnityWebRequest(SERVER_API_BASIC_ADDRESS + "/helper/all/" + inherence_id_.ToString(), "GET");
        web_request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        web_request.downloadHandler = new DownloadHandlerBuffer();

        yield return web_request.SendWebRequest();

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.Log("������ �޾ƿ���...");
                UserCatData response = JsonUtility.FromJson<UserCatData>(web_request.downloadHandler.text);
                Debug.Log("�������� �ޱ� ����...");
                for (int i = 0; i < response.catHelpers.Length; i++)
                {
                    Debug.Log(i.ToString() + "�� ������ üũ");
                    Debug.Log((response.catHelpers[i].helperId).ToString() + "����� ��");
                    if (response.catHelpers[i] != null && response.catHelpers[i].helperId>=1 && response.catHelpers[i].helperId<=GameManager.CAT_SIZE_)
                    {
                        is_unlock_cat_[response.catHelpers[i].helperId-1] = true;
                        exp_cat_[response.catHelpers[i].helperId - 1] = response.catHelpers[i].exp;
                        level_cat_[response.catHelpers[i].helperId - 1] = response.catHelpers[i].level;
                    }
                }
            }
            catch
            {
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
                Debug.Log("���� �������⵵ ����");
            }
        }

        if (is_success)
        {
            Debug.Log("����!");
            if (requestSuccededDelegate != null)
                requestSuccededDelegate();
        }
        else {
            Debug.Log("���� : " + error.message);
            if (requestFailedDelegate != null)
                requestFailedDelegate(error.message);
        }

        requesting_ = false;
    }

    private IEnumerator ChangeProfileIndexRequest(int profileNumber)
    {
        Debug.Log("������ ������ �����͸� SEND�մϴ�.");

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

        Debug.Log("�������� ������ �����͸� �޾ƿԽ��ϴ�.");

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
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("���濡 �����߽��ϴ�. �ٽ� �õ����ּ���");
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }

        requesting_ = false;
    }

    private IEnumerator BuyCatRequest(int helperPrice, int cat_idx)
    {
        Debug.Log("������ ����� ���� �����͸� SEND�մϴ�.");

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

        Debug.Log("������ ����� ���Ÿ� ��û�߽��ϴ�.");

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
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("����� ���ſ� �����߽��ϴ�.\n�ٽ� �õ����ּ��� : " + error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }

        requesting_ = false;
    }

    private IEnumerator SelectCatRequest(int[] helperIds)
    {
        Debug.Log("������ ����� ���� �����͸� SEND�մϴ�.");

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

        Debug.Log("������ ����� ���Ÿ� ��û�߽��ϴ�.");

        if (web_request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                selected_cat_ = helperIds;

                if (requestSuccededDelegate != null)
                    requestSuccededDelegate();

                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(web_request.downloadHandler.text);
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }
        else
        {
            try
            {
                ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(web_request.downloadHandler.text);
                if (requestFailedDelegate != null)
                    requestFailedDelegate("����� ���ÿ� �����߽��ϴ�.\n�ٽ� �õ����ּ��� : "+ error.ToString());
            }
            catch
            {
                if (requestFailedDelegate != null)
                    requestFailedDelegate("������ �߻��߽��ϴ�. �ٽ� �õ����ּ���");
            }
        }

        requesting_ = false;
    }



    //======================������ ��������================================
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


    //======================������ ����================================
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
        coin_ -= remove_value;//+)�ӽ� : 
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
    }

}
