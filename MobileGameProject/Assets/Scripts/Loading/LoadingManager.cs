using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static string nextScene;

    private readonly string[] TMI_LIST = { 
        "나무 한 그루는\n1년에 약 22kg의\n이산화탄소를\n흡수해요.",
    "샤워 시간 1분\n줄이면 CO2를\n연간 13kg이나\n줄일 수 있어요.",
    "하루 채식만으로도\n나무 한 그루\n심는 효과가\n있어요.",
    "에어컨보다 선풍기가\n90% 이상\n 친환경적이에요!",
    "태양광 패널 하나로\n스마트폰을\n하루 80번\n충전할 수 있어요.",
    "나무를 30년 키우면\n자동차 1대 연간\n배출량을 흡수해요.",
    "자가용 말고\n버스를 타면 \n탄소를 60%이상\n줄일 수 있어요!",
    "지하철 출퇴근은\n나무 7그루 심는\n효과가 있어요.",
    "탄소 1톤이 배출되면\n북극의 얼음\n약 3제곱미터가 사라져요.",
    "탄소가 많아지면\n폭염·홍수·가뭄이\n더 자주 찾아와요.",
    "탄소가 많아지면\n미세먼지가 심해져요",
    "CO2는 무색무취지만\n지구 온도를 끓게 해요.",
    "지금도 기온은\n120만 년 중\n최고치를 갱신 중!",
    "탄소 농도 350ppm\n넘으면 위험,\n지금은 420ppm!",
    "2050년엔 해수면이\n30cm 이상\n상승할 수 있어요.",
    "바다가 산성화돼\n물고기들이 위험해요.",
    "음식물쓰레기 CO2는\n항공기 32억 대 분량!",
    "일회용 컵 1개는\n탄소 30g!\n텀블러를 사용해요!",
    "재활용도 잘못하면\n탄소가 더 늘어요.",
    "비닐 1장을 만들면\n나무 한 그루가\n1시간 일해야 해요.",
    "지구는\n한 번 데워지면\n식히는 데\n수백 년 걸려요.",
    "멀티탭을 꺼두는 것도\n탄소를 줄일 수 있어요.",
    "인류가 1년에\n배출하는 탄소는\n무려 370억 톤!",
    "옷을 한 시즌만\n입고 버리면\n나무 한 그루를 잃어요.",
    "70억 명이\n10분만 일찍 자면\n발전소 하나가\n쉴 수 있어요.",
    "기후위기로\n1만 종 이상\n동물이 멸종위기에요!",
    "우리가 줄이는 탄소는\n다음 세대의\n기본 공기가 돼요.",
    "탄소 줄이면\n전기요금도\n아낄 수 있어요.",
    "에어컨 필터 청소도\n탄소 10%\n절약 효과!",
    "당신의 하루 선택이\n내일의 기후를 바꿔요."
};

    public SkeletonAnimation Cat;

    public TMP_Text TMI_Text;

    public Slider Gauge;
    void Start()
    {
        Debug.Log("씬 로딩매니저 스타트 함수 진입");
        Cat.initialSkinName = "Cat-" + Random.Range(1,17).ToString();
        Cat.Initialize(true);
        if (Cat.AnimationState != null)
            Cat.AnimationState.SetAnimation(0, ("idle-" + (Random.Range(1, 26)).ToString()), true);

        Gauge.value = 0.0f;
        TMI_Text.text = TMI_LIST[Random.Range(0, TMI_LIST.Length)];
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        Debug.Log(sceneName + "씬을 로드합니다");
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        Debug.Log("씬 로딩 코루틴 진입...");
        yield return new WaitForSeconds(3.0f);
        yield return null;
        Debug.Log("씬 로딩 시작...");
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                Gauge.value = Mathf.Lerp(Gauge.value, op.progress, timer);
                if (Gauge.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                Gauge.value = Mathf.Lerp(Gauge.value, 1f, timer);
                if (Gauge.value == 1.0f)
                {
                    yield return new WaitForSeconds(1.0f);
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        Debug.Log("씬 로딩 완료...");
    }
}
