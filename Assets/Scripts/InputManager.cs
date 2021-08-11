using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InputManager : MonoBehaviour
{
    public bool OnMoveLeft() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.LEFT;
        else return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public bool OnMoveRight() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.RIGHT;
        else return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public bool OnMoveDown() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.DOWN;
        else return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public int InputHorizontal() {
        if (isSmartPhone) {
            var flickDirection = ScreenInput.Instance.getFlickDirection;
            if (flickDirection == Direction.RIGHT) return 1;
            else if (flickDirection == Direction.LEFT) return -1;
            else return 0;
        } else {
            if (Input.GetKeyDown(KeyCode.RightArrow)) return 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) return -1;
            else return 0;
        }
    }

    private bool isSmartPhone {
        get { return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }
    }
}
