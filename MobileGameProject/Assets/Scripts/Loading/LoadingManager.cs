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

    private readonly string[] TMI_LIST = { "첫번째 텍스트", "두번째 텍스트" };

    public SkeletonAnimation Cat;

    public TMP_Text TMI_Text;

    public Slider Gauge;
    void Start()
    {
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
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3.0f);
        yield return null;
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
    }
}
