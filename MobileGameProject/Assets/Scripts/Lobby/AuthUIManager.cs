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


    void Start()
    {
        toastPanel.SetActive(false);
    }

    public void GotoLobby(){
        SceneManager.LoadScene("LobbyScene");
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
    public void ToggleFindpwdUI()
    {
        FindpwdUI.SetActive(!FindpwdUI.activeSelf);
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
        string nickname = nicknameInput.text.Trim();
        string id=idInput.text.Trim();
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

        StartCoroutine(SignUpRequest(email, nickname, id, password));

    }

    public void TryLogin()
    {
        string id = loginIdInput.text.Trim();
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
           
            ShowToast("아이디와 비밀번호를 입력하세요.");
            return;
        }
        StartCoroutine(LoginRequest(id, password));

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
    public void OnClickFindPassword()
    {
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
    }

    // ---------------------------
    // 🔧 서버 통신 함수 
    // ---------------------------
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

	public IEnumerator SignUpRequest(string email, string nickname, string username, string password)
	{
		SignUpData requestData = new SignUpData
		{
			username = username,
			email = email,
			nickname = nickname,
			password = password
		};

		string jsonData = JsonUtility.ToJson(requestData);

		UnityWebRequest www = new UnityWebRequest("http://3.237.76.145:8080/member/sign", "POST");
		byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
		www.uploadHandler = new UploadHandlerRaw(bodyRaw);
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.Success)
		{
			try
			{
				LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(www.downloadHandler.text);
				ShowToast($"회원가입 성공! 유저 ID: {success.id}");
				Invoke(nameof(ShowLogin), 1.5f);
			}
			catch
			{
				ShowToast("응답 파싱 오류 (성공) parsing error(success)");
			}
		}
		else
		{
			try
			{
				LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(www.downloadHandler.text);
				ShowToast($"회원가입 실패: {error.message}\n{error.description}");
			}
			catch
			{
				ShowToast("응답 파싱 오류 (실패) parsing error(failure)");
			}
		}
	}

    public IEnumerator LoginRequest(string username, string password)
    {
        LoginData loginData = new LoginData
        {
            username = username,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);

        UnityWebRequest www = new UnityWebRequest("http://3.237.76.145:8080/member/login", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                LoginSuccessResponse success = JsonUtility.FromJson<LoginSuccessResponse>(www.downloadHandler.text);
                ShowToast($"로그인 성공! 유저 ID: {success.id}");
                CloseUI();
            }
            catch
            {
                ShowToast("응답 파싱 오류 (성공)");
            }
        }
        else
        {
            try
            {
                LoginErrorResponse error = JsonUtility.FromJson<LoginErrorResponse>(www.downloadHandler.text);
                ShowToast($"Login Faliure: {error.message}\n{error.description}");
            }
            catch
            {
                ShowToast("응답 파싱 오류 (실패)");
            }
        }
    }
    
}
