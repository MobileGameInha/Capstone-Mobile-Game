using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    private readonly int SHOW_PARAM_HASH = Animator.StringToHash("SHOW");

    public GameObject[] catSet;
    private SkeletonAnimation[] movingCats;
    private SkeletonAnimation[] idleCats;
    public int textIndex;

    public Animator textSelectAnimator;
    public Animator imageShowAnimator;

    public Image showingImage;
    public Sprite[] showingImageSprites;

    public GameObject[] textCatBubbles;
    public TMP_Text[] textCatTexts;

    public GameObject[] textSelectOptions;
    public TMP_Text[] textSelectTexts;

    public string nextSceneName;

    public AudioSource effectAudioSource;
    public AudioSource backgroundAudioSource;

    public AudioClip talkClip;
    public AudioClip buttonClip;
    public AudioClip[] effectClips;
    public AudioClip[] backgroundClips;

    public GameObject SkipPanel;

    public bool reSee = false;

    private Dictionary<int, TextSet> chat_dictionary;

    private int now_index_ = 0;
    private TextSet now_text_set_;
    private bool ready_to_next_cut_ = false;
    private bool is_selecting_ = false;


    private string showing_text_ = "";
    private bool is_skip_=false;
    private bool end_cut_scene_ = false;


    private void Awake()
    {
        SkipPanel.SetActive(false);

        textSelectAnimator.SetBool(SHOW_PARAM_HASH, false);
        movingCats = new SkeletonAnimation[catSet.Length];
        idleCats = new SkeletonAnimation[catSet.Length];
        for (int i = 0; i < catSet.Length; i++)
        {
            SkeletonAnimation[] animations = catSet[i].GetComponentsInChildren<SkeletonAnimation>(true);
            foreach (SkeletonAnimation item in animations)
            {
                if (item.CompareTag("Idle"))
                {
                    idleCats[i] = item;
                    idleCats[i].Initialize(true);
                } 
                else if (item.CompareTag("Moving"))
                {
                    movingCats[i] = item;
                    movingCats[i].Initialize(true);
                }
            }
        }
    }

    private void Start()
    {
        StartCutScene();
    }

    private void StartCutScene() 
    {
        chat_dictionary = CSVOpener.OpenFileChat(textIndex);
        if (chat_dictionary == null) { LoadingManager.LoadScene("LobbyScene"); }
        else
        {
            StartBGM(0);
            now_index_ = -1;
            ready_to_next_cut_ = true;
            is_selecting_ = false;
            is_skip_ = false;
            end_cut_scene_ = false;
            NextCutScene();
        }
    }

    public void NextCutScene() 
    {
        if (end_cut_scene_) { EndCutScene(); }

        if (!ready_to_next_cut_ || is_selecting_) {
            if (!is_skip_) { is_skip_ = true; }
            return; 
        }

        ready_to_next_cut_ = false;
        is_skip_ = false;

        textSelectAnimator.SetBool(SHOW_PARAM_HASH, false);

        now_index_++;
        now_text_set_ = chat_dictionary[now_index_];

        if (now_text_set_.have_end_event_ && now_text_set_.end_event_index_ >= 1000)
        {
            EndEvent(now_text_set_.end_event_index_);
        }

        is_selecting_ = now_text_set_.have_selection_;

        if (now_text_set_.have_sound_)
        {
            StartEffectSound(now_text_set_.sound_index_);
        }

        idleCats[now_text_set_.talking_index_].AnimationState.SetAnimation(0, ("idle-" + (now_text_set_.sprite_index_).ToString()), true);

        showing_text_ = now_text_set_.text_;

            for (int i = 0; i < textCatBubbles.Length; i++)
            {
                if (now_text_set_.talking_index_==i)
                {
                    textCatBubbles[i].SetActive(true);
                    textCatTexts[i].text = "";
                }
                else 
                {
                    textCatBubbles[i].SetActive(false);
                }
            }

        if (is_selecting_)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < now_text_set_.selection_count_)
                {
                    textSelectOptions[i].SetActive(true);
                    textSelectTexts[i].text = now_text_set_.select_text_[i];
                }
                else
                {
                    textSelectOptions[i].SetActive(false);
                }
            }
        }

        StartCoroutine(ChattingCoroutine());

    }

    private IEnumerator ChattingCoroutine() 
    {
        for (int i = 0; i < showing_text_.Length; i++)
        {
            if (is_skip_)
            {
                yield return new WaitForSeconds(0.03f);
                OnClickTalkSound();
            }
            else
            {
                yield return new WaitForSeconds(0.075f);
                OnClickTalkSound();
            }
            if (showing_text_[i] == ':')
            {
                textCatTexts[now_text_set_.talking_index_].text += "\n";
            }
            else
            {
                textCatTexts[now_text_set_.talking_index_].text += showing_text_[i];
            }
        }

        if (now_text_set_.have_end_event_ && now_text_set_.end_event_index_<1000)
        {
            EndEvent(now_text_set_.end_event_index_);
        }
        else
        {
            ready_to_next_cut_ = true;

            if (is_selecting_)
            {
                is_selecting_ = false;
                textSelectAnimator.SetBool(SHOW_PARAM_HASH, true);
            }
        }
    }

    public void EndCutScene() 
    {
        PlayerPrefs.SetInt("Stage_" + textIndex.ToString(), 1);
        if (reSee)
        {
            LoadingManager.LoadScene("LobbyScene");
        }
        else {
            LoadingManager.LoadScene(nextSceneName);
        }
    }

    public void SelectEvent(int button_index) 
    {
        switch (now_text_set_.select_event_index_[button_index])
        {
            case 0: //단순 넥스트
                is_selecting_ = false;
                NextCutScene();
                break;
            case 1: //점프
                is_selecting_ = false;
                now_index_ += now_text_set_.jump_index_;
                NextCutScene();
                break;
            case 2: //단순 넥스트 + 배경음 삭제
                is_selecting_ = false;
                StopBGM();
                NextCutScene();
                break;
            default:
                is_selecting_ = false;
                NextCutScene();
                break;
        }
    }

    public void EndEvent(int idx)  //1000번대는 시작시
    {
        switch (idx)
        {
            case 0: //END
                end_cut_scene_ = true;
                ready_to_next_cut_ = true;

                if (is_selecting_)
                {
                    is_selecting_ = false;
                    textSelectAnimator.SetBool(SHOW_PARAM_HASH, true);
                }
                break;
            case 1://for Cutscene 1,2
                StartCoroutine(MoveToPositionX(0, -150.0f)); 
                break;
            case 2://JUMP
                now_index_ += now_text_set_.jump_index_;

                ready_to_next_cut_ = true;

                if (is_selecting_)
                {
                    is_selecting_ = false;
                    textSelectAnimator.SetBool(SHOW_PARAM_HASH, true);
                }
                break;
            case 3://for Cutscene 4
                idleCats[1].GetComponent<RectTransform>().localScale = new Vector3(50, 50, 100);
                StartCoroutine(MoveToPositionX(2, 175.0f));
                break;
            case 1000://이미지 제거
                    imageShowAnimator.SetBool(SHOW_PARAM_HASH, false);
                    break;
            case 1001://이미지 생성
            case 1002:
            case 1003:
            case 1004:
                showingImage.sprite = showingImageSprites[idx - 1001];
                    imageShowAnimator.SetBool(SHOW_PARAM_HASH, true);
                    break;
            case 2000://for Cutscene 2
                StopBGM();
                imageShowAnimator.SetBool(SHOW_PARAM_HASH, false);
                    StartCoroutine(MoveToPositionX(1, 150.0f, false));
                    break;
            case 2001://for Cutscene 1
                imageShowAnimator.SetBool(SHOW_PARAM_HASH, false);
                StartCoroutine(MoveToPositionX(0, 0.0f, false));
                break;
            case 3000://BGM종료
                StopBGM();
                break;
            case 3001://BGM시작
            case 3002:
            case 3003:
                StartBGM(idx - 3000);
                break;
            default:
                    ready_to_next_cut_ = true;

                    if (is_selecting_)
                    {
                        is_selecting_ = false;
                        textSelectAnimator.SetBool(SHOW_PARAM_HASH, true);
                    }
                    break;
            }
    }

    private void OnClickTalkSound() 
    {
        effectAudioSource.PlayOneShot(talkClip);
    }

    public void OnClickButtonSound()
    {
        effectAudioSource.PlayOneShot(buttonClip);
    }

    public void StartEffectSound(int idx) 
    {
        effectAudioSource.PlayOneShot(effectClips[idx]);
    }

    public void StartBGM(int idx) 
    {
        backgroundAudioSource.clip = backgroundClips[idx];
        backgroundAudioSource.Play();
    }

    public void StopBGM()
    {
        backgroundAudioSource.Stop();
    }

    public void OnClickSkip() {
        SkipPanel.SetActive(true);
    }

    public void OnClickSkipNo()
    {
        SkipPanel.SetActive(false);
    }

    public void OnClickSkipYes()
    {
        EndCutScene();
    }



    //===============이벤트=========================
    private IEnumerator MoveToPositionX(int idx, float x, bool go_to_next_chat = true)
    {
        RectTransform CatRT = catSet[idx].GetComponent<RectTransform>();
        RectTransform MovingCatRT = movingCats[idx].GetComponent<RectTransform>();

        idleCats[idx].gameObject.SetActive(false);
        movingCats[idx].gameObject.SetActive(true);

        MovingCatRT.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        if (CatRT.anchoredPosition.x < x)
        {
            MovingCatRT.localScale = new Vector3(50.0f, 50.0f, 100.0f);
        }
        else
        {
            MovingCatRT.localScale = new Vector3(50.0f, -50.0f, 100.0f);
        }

        while (true)
        {
            float delta_time = Time.deltaTime * 100.0f;

            if (CatRT.anchoredPosition.x < x)
            {
                if (CatRT.anchoredPosition.x + delta_time >= x)
                {
                    CatRT.anchoredPosition = new Vector3(x, CatRT.anchoredPosition.y);
                    break;
                }
                else {
                    CatRT.anchoredPosition = new Vector3(CatRT.anchoredPosition.x + delta_time, CatRT.anchoredPosition.y);
                }
            }
            else
            {
                if (CatRT.anchoredPosition.x - delta_time <= x)
                {
                    CatRT.anchoredPosition = new Vector3(x, CatRT.anchoredPosition.y);
                    break;
                }
                else
                {
                    CatRT.anchoredPosition = new Vector3(CatRT.anchoredPosition.x - delta_time, CatRT.anchoredPosition.y);
                }
            }

            yield return null;
        }

        idleCats[idx].gameObject.SetActive(true);
        movingCats[idx].gameObject.SetActive(false);

        if (go_to_next_chat)
        {
            ready_to_next_cut_ = true;

            if (is_selecting_)
            {
                is_selecting_ = false;
                textSelectAnimator.SetBool(SHOW_PARAM_HASH, true);
            }

            NextCutScene();
        }
    }
}


public struct TextSet
{
    public int talking_index_; 
    public string text_;

    public int sprite_index_;

    public bool have_selection_;
    public int selection_count_;
    public string[] select_text_;
    public int[] select_event_index_;

    public bool have_sound_;
    public int sound_index_;

    public bool have_end_event_;
    public int end_event_index_;

    public int jump_index_;
}

public class CSVOpener
{


    public static Dictionary<int, TextSet> OpenFileChat(int idx)
    {
        StreamReader streamReader = new StreamReader(Application.streamingAssetsPath +"/TextSet/text" + idx.ToString() + ".csv");
        if (streamReader == null) { Debug.Log("파일 읽기 실패!"); return null; }

        Dictionary<int, TextSet> tmp = new Dictionary<int, TextSet>();

        int count = 0;

        string str = streamReader.ReadLine();

        while (true)
        {
            str = streamReader.ReadLine();

            if (str == null) break;

            else
            {
                Debug.Log(count.ToString()+"번째 라인 읽기");

                var datas = str.Split(',');
                TextSet textSet = new TextSet();
                textSet.talking_index_ = int.Parse(datas[0]);
                textSet.text_ = datas[1];
                textSet.sprite_index_ = int.Parse(datas[2]);

                textSet.have_selection_ = bool.Parse(datas[3]);
                if (textSet.have_selection_)
                {
                    textSet.selection_count_ = int.Parse(datas[4]);

                    textSet.select_text_ = new string[3];
                    textSet.select_event_index_ = new int[3];

                    textSet.select_text_[0] = datas[5];
                    textSet.select_text_[1] = datas[6];
                    textSet.select_text_[2] = datas[7];

                    textSet.select_event_index_[0] = int.Parse(datas[8]);
                    textSet.select_event_index_[1] = int.Parse(datas[9]);
                    textSet.select_event_index_[2] = int.Parse(datas[10]);
                }

                textSet.have_sound_ = bool.Parse(datas[11]);
                if (textSet.have_sound_)
                {
                    textSet.sound_index_ = int.Parse(datas[12]);
                }

                textSet.have_end_event_ = bool.Parse(datas[13]);
                if (textSet.have_end_event_)
                {
                    textSet.end_event_index_ = int.Parse(datas[14]);
                }

                textSet.jump_index_ = int.Parse(datas[15]);

                tmp.Add(count++, textSet);
            }
        }
        streamReader.Close();
        return tmp;
    }

}