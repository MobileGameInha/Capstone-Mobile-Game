using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonRotator : MonoBehaviour
{
    public RectTransform[] ButtonPositions;

    public void RotateButtons() {
        if (ButtonPositions.Length <= 0) { return; }
        Vector2 position = ButtonPositions[0].anchoredPosition;
        for (int i = 0; i < ButtonPositions.Length-1; i++)
        {
            ButtonPositions[i].anchoredPosition = ButtonPositions[i + 1].anchoredPosition;
        }
        ButtonPositions[ButtonPositions.Length - 1].anchoredPosition = position;
    }
}
