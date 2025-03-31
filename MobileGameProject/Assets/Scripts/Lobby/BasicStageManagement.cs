using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicStageManagement : MonoBehaviour
{
    public GameObject StageSelectPack;
    public GameObject StagesPack;

    public RectTransform CanvasTransform;
    public RectTransform[] StageTransforms;

    public RectTransform RightPosition;
    public RectTransform LeftPosition;


    public int MaxStage;
    public float speed = 20.0f;
    public CanvasScaler canvasScaler;

    private Vector2 MainPosition;

    private bool isSweepRight = false;
    private bool isSweepLeft = false;

    private int currentStage = 0;

    private RectTransform CurrentStageTransform;
    private RectTransform RightStageTransform;
    private RectTransform LeftStageTransform;

    private void Awake()
    {
        MainPosition = StageTransforms[0].position;
        float scaleFactor = Screen.width / canvasScaler.referenceResolution.x;
        isSweepRight = false;
        isSweepLeft = false;

        currentStage = PlayerPrefs.GetInt("Stage", 0);

        for (int i = 0; i < StageTransforms.Length; i++)
        {
            StageTransforms[i].position = LeftPosition.position;
        }
        ResetStagePositions();

        StagesPack.SetActive(false);
        StageSelectPack.SetActive(true);
    }


    private void Update()
    {
        if (isSweepRight)
        {
            CurrentStageTransform.position = 
                new Vector2(Mathf.Clamp(CurrentStageTransform.position.x + speed * Time.deltaTime, CurrentStageTransform.position.x, RightPosition.position.x), CurrentStageTransform.position.y);

            LeftStageTransform.position =
                new Vector2(Mathf.Clamp(LeftStageTransform.position.x + speed * Time.deltaTime, LeftStageTransform.position.x, MainPosition.x), LeftStageTransform.position.y);

            if (LeftStageTransform.position.x >= MainPosition.x) 
            {
                isSweepRight = false;
                ResetStagePositions();
            }
        }

        if (isSweepLeft)
        {
            CurrentStageTransform.position =
                new Vector2(Mathf.Clamp(CurrentStageTransform.position.x - speed * Time.deltaTime, LeftPosition.position.x, CurrentStageTransform.position.x), CurrentStageTransform.position.y);

            RightStageTransform.position =
                new Vector2(Mathf.Clamp(RightStageTransform.position.x - speed * Time.deltaTime, MainPosition.x, RightStageTransform.position.x), RightStageTransform.position.y);

            if (RightStageTransform.position.x <= MainPosition.x)
            {
                isSweepLeft = false;
                ResetStagePositions();
            }
        }


    }

    public void OnClickMoveToStagesPack(int idx)
    {
        currentStage = idx;
        ResetStagePositions();

        StagesPack.SetActive(true);
        StageSelectPack.SetActive(false);
    }

    public void OnClickMoveToStageSelectPack()
    {
        StagesPack.SetActive(false);
        StageSelectPack.SetActive(true);
    }

    public void OnClickMoveToStage(int idx) {
        SceneManager.LoadScene("Stage_" + idx.ToString());
    }


    public void OnClickMoveStageButton(bool isRight) {
        if (isSweepRight || isSweepLeft)
        {
            return;
        }

        if (isRight)
        {
            currentStage--;
            if (currentStage < 0)
            {
                currentStage = MaxStage - 1;
                PlayerPrefs.SetInt("Stage", currentStage);
            }
            isSweepRight = true;
        }
        else
        {
            
            currentStage++;
            if (currentStage >= MaxStage)
            {
                currentStage = 0;
                PlayerPrefs.SetInt("Stage", currentStage);
            }
            isSweepLeft = true;
        }
    }

    private void ResetStagePositions()
    {
        for (int i = 0; i < MaxStage; i++)
        {
            StageTransforms[i].position = LeftPosition.position;
        }

        CurrentStageTransform = StageTransforms[currentStage];

        if (currentStage == 0)
        {
            RightStageTransform = StageTransforms[1];
            LeftStageTransform = StageTransforms[MaxStage - 1];
        }
        else if (currentStage == MaxStage - 1)
        {
            RightStageTransform = StageTransforms[0];
            LeftStageTransform = StageTransforms[MaxStage - 2];
        }
        else
        {
            RightStageTransform = StageTransforms[currentStage+1];
            LeftStageTransform = StageTransforms[currentStage - 1];
        }

        CurrentStageTransform.position = MainPosition;
        RightStageTransform.position = RightPosition.position;
        LeftStageTransform.position = LeftPosition.position;

    }


}
