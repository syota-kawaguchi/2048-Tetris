using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InputManager : MonoBehaviour
{
    public bool OnMoveLeft() {
        if (isSmartPhone) {
            var settings = SettingsController.Instance.settings;
            if (settings.moveVertical == SettingsController.FLICK) return ScreenInput.Instance.getFlickDirection == Direction.LEFT;
            if (settings.moveVertical == SettingsController.SWIPE) return ScreenInput.Instance.getSwipeDirection == Direction.LEFT;
            if (settings.moveVertical == SettingsController.TAP) return false; //Todo
            return false;
        }
        else return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public bool OnMoveRight() {
        if (isSmartPhone) {
            var settings = SettingsController.Instance.settings;
            if (settings.moveVertical == SettingsController.FLICK) return ScreenInput.Instance.getFlickDirection == Direction.RIGHT;
            if (settings.moveVertical == SettingsController.SWIPE) return ScreenInput.Instance.getSwipeDirection == Direction.RIGHT;
            if (settings.moveVertical == SettingsController.TAP) return false; //Todo
            return false;
        }
        else return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public bool OnMoveDown() {
        if (isSmartPhone) return ScreenInput.Instance.getFlickDirection == Direction.DOWN;
        else return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public int InputHorizontal() {
        var moveWay = SettingsController.Instance.settings.moveHorizontal;
        if (isSmartPhone) {
            switch (moveWay) {
                case SettingsController.FLICK:
                    var flickDirection = ScreenInput.Instance.getFlickDirection;
                    if (flickDirection == Direction.RIGHT) return 1;
                    else if (flickDirection == Direction.LEFT) return -1;
                    else return 0;
                case SettingsController.SWIPE:
                    var swipeDirection = ScreenInput.Instance.getSwipeDirection;
                    if (swipeDirection == Direction.RIGHT) return 1;
                    else if (swipeDirection == Direction.LEFT) return -1;
                    else return 0;
                case SettingsController.TAP:
                    var touchPos = ScreenInput.Instance.getTouchPos;

                    return 0;
                default:
                    return 0;
            }
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
