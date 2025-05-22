using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class AuthUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject LOG_IN;
    public GameObject SIGN_UP;

    public GameObject CONNECT;
    public GameObject START_BTN;

    [Header("Remember Button")]
    public GameObject REM_ON;

    private bool ison;

    [Header("Sign Up Inputs")]
    public TMP_InputField emailInput;
    public TMP_InputField nicknameInput;
    public TMP_InputField idInput;
    public TMP_InputField passwordInput;

    [Header("Login Inputs")]
    public TMP_InputField loginIdInput;
    public TMP_InputField loginPasswordInput;

    [Header("Toast UI")]
    public TMP_Text toastText;
    public GameObject toastPanel;
    [Header("Find password UI")]
    public GameObject FindpwdUI;
    public TMP_InputField idInput_find;
    public TMP_InputField emailInput_find;
    public TMP_Text resultText;

    private bool is_requesting_ = false;
    private bool login_request_ = false;
    private bool signup_request_ = false;

    void Awake()
    {
        DataManager.dataManager.requestSuccededDelegate += SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate += FailRequestEvent;
        toastPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        DataManager.dataManager.requestSuccededDelegate -= SuccessRequestEvent;
        DataManager.dataManager.requestFailedDelegate -= FailRequestEvent;
    }



    public void GotoLobby()
    {
        if (is_requesting_) { return; }

        LoadingManager.LoadScene("LobbyScene");
        //SceneManager.LoadScene("LobbyScene"); 동기식...
    }


    public void Remember_click() {
        if (is_requesting_) { return; }

        if (ison) {
            REM_ON.SetActive(false);
            ison = false;
        }
        else {
            REM_ON.SetActive(true);
            ison = true;
        }

    }
    public void ToggleFindpwdUI()
    {
        if (is_requesting_) { return; }

        FindpwdUI.SetActive(!FindpwdUI.activeSelf);
    }
    public void ShowLogin()
    {
        if (is_requesting_) { return; }

        LOG_IN.SetActive(true);
        SIGN_UP.SetActive(false);
    }

    public void ShowSignUp()
    {
        if (is_requesting_) { return; }

        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(true);
    }

    public void CloseUI()
    {
        if (is_requesting_) { return; }

        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(false);
        CONNECT.SetActive(false);
        START_BTN.SetActive(true);

    }

    public void TrySignUp()
    {
        if (is_requesting_ || DataManager.dataManager.GetIsRequesting()) { return; }

        string email = emailInput.text.Trim();
        string nickname = nicknameInput.text.Trim();
        string id = idInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            ShowToast("형식에 맞는 이메일이 입력되지 않았습니다.");
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            ShowToast("닉네임을 입력하세요.");
            return;
        }
        if (string.IsNullOrEmpty(id))
        {
            ShowToast("아이디를 입력하세요.");
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            ShowToast("비밀번호는 6자 이상이어야 합니다.");
            return;
        }

        is_requesting_ = true;
        signup_request_ = true;
        DataManager.dataManager.SendSignUpRequest(email, nickname, id, password);
    }

    public void TryLogin()
    {
        if (is_requesting_ || DataManager.dataManager.GetIsRequesting()) { return; }

        string id = loginIdInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            ShowToast("아이디와 비밀번호를 입력하세요.");
            return;
        }

        is_requesting_ = true;
        login_request_ = true;
        DataManager.dataManager.SendLoginRequest(id, password);
    }

    public void ShowToast(string message)
    {
        toastText.text = message;
        StopAllCoroutines();
        StartCoroutine(ToastRoutine());
    }

    private IEnumerator ToastRoutine()
    {
        toastPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        toastPanel.SetActive(false);
    }
    public void OnClickFindPassword()
    {
        if (is_requesting_ || DataManager.dataManager.GetIsRequesting()) { return; }

        string id = idInput_find.text.Trim();
        string email = emailInput_find.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
        {
            ShowToast("아이디와 이메일을 모두 입력해주세요.");
            return;
        }
        if (!email.Contains("@"))
        {
            ShowToast("형식에 맞는 이메일이 입력되지 않았습니다.");
            return;
        }

        resultText.text = "Email and Id Mismatch";
        //!!!!++++++++)))))))) : 락걸고 요청걸기
    }


    private void SuccessRequestEvent()
    {
        Debug.Log("서버요청 성공 이벤트 발생");

        if (is_requesting_)
        {
            if (signup_request_)
            {
                is_requesting_ = false;
                signup_request_ = false;
                ShowToast($"회원가입 성공! 로그인 하세요!");
                ShowLogin();
            }
            else if (login_request_)
            {
                Debug.Log("서버요청 성공 이벤트 발생 :  로그인");
                is_requesting_ = false;
                login_request_ = false;
                ShowToast($"로그인 성공! 로그인 하세요!");
                CloseUI();
            }
        }
    }

    private void FailRequestEvent(string err)
    {
        if (is_requesting_)
        {
            is_requesting_ = false;
            signup_request_ = false;
            login_request_ = false;
            ShowToast(err);
        }
    }

    public void CloseLogin()
    {
        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(false);
    }
}
