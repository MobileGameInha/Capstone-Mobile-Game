using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserButton : MonoBehaviour
{
    public ArrowDirection direction_;

    public void OnClickButton() {
        GameManager.gameManager.OnClickButton(direction_);
    }


}
