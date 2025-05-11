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

    private bool request_lock_ = false;

    void Awake()
    {
        toastPanel.SetActive(false);
    }


    public void Unlock() {
        request_lock_ = false;
    }


    public void GotoLobby(){
        if (request_lock_) { return; }

        SceneManager.LoadScene("LobbyScene");
    }

    public void Remember_click(){
        if (request_lock_) { return; }

        if (ison){
            REM_ON.SetActive(false);
            ison=false;
        }
        else{
            REM_ON.SetActive(true);
            ison=true;
        }
        
    }
    public void ToggleFindpwdUI()
    {
        if (request_lock_) { return; }

        FindpwdUI.SetActive(!FindpwdUI.activeSelf);
    }
    public void ShowLogin()
    {
        if (request_lock_) { return; }
        emailInput.text = "";
        nicknameInput.text = "";
        idInput.text = "";
        passwordInput.text = "";
        
        LOG_IN.SetActive(true);
        SIGN_UP.SetActive(false);
    }

    public void ShowSignUp()
    {
        if (request_lock_) { return; }

        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(true);
    }
    
    public void CloseUI()
    {
        if (request_lock_) { return; }

        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(false);
        CONNECT.SetActive(false);
        START_BTN.SetActive(true);

    }
    public void CloseLogin(){
        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(false);
    }

    public void TrySignUp()
    {
        if (request_lock_) { return; }
        request_lock_ = true;


        string email = emailInput.text.Trim();
        string nickname = nicknameInput.text.Trim();
        string id=idInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            ShowToast("형식에 맞는 이메일이 입력되지 않았습니다.");
            Unlock();
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            ShowToast("닉네임을 입력하세요.");
            Unlock();
            return;
        }
        if (string.IsNullOrEmpty(id))
        {
            ShowToast("아이디를 입력하세요.");
            Unlock();
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            ShowToast("비밀번호는 6자 이상이어야 합니다.");
            Unlock();
            return;
        }

        DataManager.dataManager.SendSignUpRequest(this, email, nickname, id, password);
    }

    public void TryLogin()
    {
        if (request_lock_) { return; }
        request_lock_ = true;

        string id = loginIdInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            ShowToast("아이디와 비밀번호를 입력하세요.");
            Unlock();
            return;
        }
        DataManager.dataManager.SendLoginRequest(this, id, password);
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
        if (request_lock_) { return; }
        request_lock_ = true;

        string id = idInput_find.text.Trim();
        string email = emailInput_find.text.Trim();

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
        {
            ShowToast("아이디와 이메일을 모두 입력해주세요.");
            Unlock();
            return;
        }
        if (!email.Contains("@"))
        {
            ShowToast("형식에 맞는 이메일이 입력되지 않았습니다.");
            Unlock();
            return;
        }

        resultText.text = "Email and Id Mismatch";
        Unlock(); //!!!!임시코드 : 데이터 매니저에서 락 풀어야 함
    }

}
