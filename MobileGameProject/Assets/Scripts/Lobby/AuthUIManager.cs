using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
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
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    [Header("Login Inputs")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Toast UI")]
    public TMP_Text toastText;
    public GameObject toastPanel;

    private const string SERVER_URL = "https://yourbackend.com/api"; // 예시용

    void Start()
    {
        toastPanel.SetActive(false);
    }

    public void GotoRobby(){
        SceneManager.LoadScene("RobbyScene");
    }

    public void Remember_click(){
        if(ison){
            REM_ON.SetActive(false);
            ison=false;
        }
        else{
            REM_ON.SetActive(true);
            ison=true;
        }
        
    }

    public void ShowLogin()
    {
        LOG_IN.SetActive(true);
        SIGN_UP.SetActive(false);
    }

    public void ShowSignUp()
    {
        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(true);
    }
    
    public void CloseUI()
    {
        LOG_IN.SetActive(false);
        SIGN_UP.SetActive(false);
        CONNECT.SetActive(false);
        START_BTN.SetActive(true);

    }

    public void TrySignUp()
    {
        string email = emailInput.text.Trim();
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            ShowToast("유효한 이메일을 입력하세요.");
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            ShowToast("사용자 이름을 입력하세요.");
            return;
        }

        if (string.IsNullOrEmpty(password) || password.Length < 6)
        {
            ShowToast("비밀번호는 6자 이상이어야 합니다.");
            return;
        }

        // 추후 서버 통신: StartCoroutine(SignUpRequest(email, username, password));
        ShowToast("회원가입 완료! 로그인 화면으로 이동합니다.");
        Invoke(nameof(ShowLogin), 1.5f);
    }

    public void TryLogin()
    {
        string username = loginUsernameInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
           
            ShowToast("아이디와 비밀번호를 입력하세요.");
            return;
        }
        CloseUI();

        // 추후 서버 통신: StartCoroutine(LoginRequest(username, password));
        ShowToast("로그인 시도 중... (서버 연동 예정)");
    }

    private void ShowToast(string message)
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

    // ---------------------------
    // 🔧 서버 통신 함수 (구현 예정)
    // ---------------------------

    IEnumerator SignUpRequest(string email, string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"{SERVER_URL}/signup", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                ShowToast("회원가입 성공!");
                Invoke(nameof(ShowLogin), 1.5f);
            }
            else
            {
                ShowToast("회원가입 실패: " + www.error);
            }
        }
    }

    IEnumerator LoginRequest(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post($"{SERVER_URL}/login", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                ShowToast("로그인 성공!");
                // 로그인 성공 후 다음 씬으로 이동하거나 처리
            }
            else
            {
                ShowToast("로그인 실패: " + www.error);
            }
        }
    }
}
