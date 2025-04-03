using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonRotator : MonoBehaviour
{
    public Transform[] ButtonPositions;

    public void RotateButtons() {
        if (ButtonPositions.Length <= 0) { return; }
        Vector3 position = ButtonPositions[0].position;
        for (int i = 0; i < ButtonPositions.Length-1; i++)
        {
            ButtonPositions[i].position = ButtonPositions[i + 1].position;
        }
        ButtonPositions[ButtonPositions.Length - 1].position = position;
    }
}
