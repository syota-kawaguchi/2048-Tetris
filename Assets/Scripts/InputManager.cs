using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float leftMostLower;
    [SerializeField]
    private float leftLower;
    [SerializeField]
    private float centerLower;
    [SerializeField]
    private float rightLower;
    [SerializeField]
    private float rightMostLower;
    [SerializeField]
    private float rightMostUpper;

    public bool OnMoveDown() {
        var settings = SettingsController.Instance.settings;
        if (isSmartPhone) {
            if (settings.moveVertical == (int)Operation.FlickDown) return ScreenInput.Instance.getFlickDirection == Direction.DOWN;
            if (settings.moveVertical == (int)Operation.DoubleTap) return ScreenInput.Instance.DoubleTap();
            return false;
        }
        else return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public int InputHorizontal() {
        var moveWay = (Operation)SettingsController.Instance.settings.moveHorizontal;
        if (isSmartPhone) {
            switch (moveWay) {
                case Operation.Flick:
                    var flickDirection = ScreenInput.Instance.getFlickDirection;
                    if (flickDirection == Direction.RIGHT) return 1;
                    else if (flickDirection == Direction.LEFT) return -1;
                    else return 0;
                case Operation.Swipe:
                    var swipeDirection = ScreenInput.Instance.getSwipeDirection;
                    if (swipeDirection == Direction.RIGHT) return 1;
                    else if (swipeDirection == Direction.LEFT) return -1;
                    else return 0;
                case Operation.Tap:
                    var touchPos = ScreenInput.Instance.getTouchPos;
                    if (leftMostLower <= touchPos.x && touchPos.x < leftLower) return -2;
                    else if (touchPos.x <= leftLower) return -1;
                    else if (touchPos.x <= centerLower) return 0;
                    else if (touchPos.x <= rightLower) return 1;
                    else if (touchPos.x <= rightMostUpper) return 2;
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
        get { return true; } //{ return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }
    }
}
