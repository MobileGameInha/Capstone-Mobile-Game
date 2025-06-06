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
        "���� �� �׷��\n1�⿡ �� 22kg��\n�̻�ȭź�Ҹ�\n����ؿ�.",
    "���� �ð� 1��\n���̸� CO2��\n���� 13kg�̳�\n���� �� �־��.",
    "�Ϸ� ä�ĸ����ε�\n���� �� �׷�\n�ɴ� ȿ����\n�־��.",
    "���������� ��ǳ�Ⱑ\n90% �̻�\n ģȯ�����̿���!",
    "�¾籤 �г� �ϳ���\n����Ʈ����\n�Ϸ� 80��\n������ �� �־��.",
    "������ 30�� Ű���\n�ڵ��� 1�� ����\n���ⷮ�� ����ؿ�.",
    "�ڰ��� ����\n������ Ÿ�� \nź�Ҹ� 60%�̻�\n���� �� �־��!",
    "����ö �������\n���� 7�׷� �ɴ�\nȿ���� �־��.",
    "ź�� 1���� ����Ǹ�\n�ϱ��� ����\n�� 3�������Ͱ� �������.",
    "ź�Ұ� ��������\n������ȫ����������\n�� ���� ã�ƿͿ�.",
    "ź�Ұ� ��������\n�̼������� ��������",
    "CO2�� ������������\n���� �µ��� ���� �ؿ�.",
    "���ݵ� �����\n120�� �� ��\n�ְ�ġ�� ���� ��!",
    "ź�� �� 350ppm\n������ ����,\n������ 420ppm!",
    "2050�⿣ �ؼ�����\n30cm �̻�\n����� �� �־��.",
    "�ٴٰ� �꼺ȭ��\n�������� �����ؿ�.",
    "���Ĺ������� CO2��\n�װ��� 32�� �� �з�!",
    "��ȸ�� �� 1����\nź�� 30g!\n�Һ��� ����ؿ�!",
    "��Ȱ�뵵 �߸��ϸ�\nź�Ұ� �� �þ��.",
    "��� 1���� �����\n���� �� �׷簡\n1�ð� ���ؾ� �ؿ�.",
    "������\n�� �� ��������\n������ ��\n���� �� �ɷ���.",
    "��Ƽ���� ���δ� �͵�\nź�Ҹ� ���� �� �־��.",
    "�η��� 1�⿡\n�����ϴ� ź�Ҵ�\n���� 370�� ��!",
    "���� �� ����\n�԰� ������\n���� �� �׷縦 �Ҿ��.",
    "70�� ����\n10�и� ���� �ڸ�\n������ �ϳ���\n�� �� �־��.",
    "���������\n1�� �� �̻�\n������ �������⿡��!",
    "�츮�� ���̴� ź�Ҵ�\n���� ������\n�⺻ ���Ⱑ �ſ�.",
    "ź�� ���̸�\n�����ݵ�\n�Ƴ� �� �־��.",
    "������ ���� û�ҵ�\nź�� 10%\n���� ȿ��!",
    "����� �Ϸ� ������\n������ ���ĸ� �ٲ��."
};

    public SkeletonAnimation Cat;

    public TMP_Text TMI_Text;

    public Slider Gauge;
    void Start()
    {
        Debug.Log("�� �ε��Ŵ��� ��ŸƮ �Լ� ����");
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
        Debug.Log(sceneName + "���� �ε��մϴ�");
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        Debug.Log("�� �ε� �ڷ�ƾ ����...");
        yield return new WaitForSeconds(3.0f);
        yield return null;
        Debug.Log("�� �ε� ����...");
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
        Debug.Log("�� �ε� �Ϸ�...");
    }
}
