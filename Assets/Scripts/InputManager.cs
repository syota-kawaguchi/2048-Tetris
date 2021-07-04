using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool onMoveLeft() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.LEFT;
        else return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public bool onMoveRight() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.RIGHT;
        else return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public bool onMoveDown() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.DOWN;
        else return Input.GetKeyDown(KeyCode.DownArrow);
    }

    private bool isSmartPhone {
        get { return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }
    }
}
