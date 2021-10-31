using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private float leftMostLower = -0.6f;
    [SerializeField]
    private float leftLower = 0.5f;
    [SerializeField]
    private float centerLower = 1.5f;
    [SerializeField]
    private float rightLower = 2.5f;
    [SerializeField]
    private float rightMostLower = 3.5f;
    [SerializeField]
    private float rightMostUpper = 4.6f;

    public bool OnMoveDown() {
        var settings = SettingsController.Instance.settings;
        if (isSmartPhone) {
            if (settings.moveVertical == (int)Operation.FlickDown) return ScreenInput.Instance.getFlickDirection == Direction.DOWN;
            if (settings.moveVertical == (int)Operation.DoubleTap) return ScreenInput.Instance.DoubleTap();
            return false;
        }
        else return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public int InputHorizontal(Panel panel) {
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
                    if (Input.touchCount > 0 || Input.GetMouseButton(0)) {
                        var touchLane = ConvertTouchPosToLane(ScreenInput.Instance.getTouchPos);
                        var currentLane = panel.getCurrentLane;
                        return touchLane - currentLane;
                    }
                    else return 0;
                default:
                    return 0;
            }
        } else {
            if (Input.GetKeyDown(KeyCode.RightArrow)) return 1;
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) return -1;
            else return 0;
        }
    }

    private int ConvertTouchPosToLane(Vector2 touchPosPixel) {
        var touchPosWorld = Camera.main.ScreenToWorldPoint(touchPosPixel);
        int rtv = 0;
        if (leftMostLower <= touchPosWorld.x && touchPosWorld.x < leftLower) rtv = 1;
        else if (touchPosWorld.x <= centerLower) rtv = 2;
        else if (touchPosWorld.x <= rightLower) rtv = 3;
        else if (touchPosWorld.x <= rightMostLower) rtv = 4;
        else if (touchPosWorld.x <= rightMostUpper) rtv = 5;
        return rtv;
    }

    private bool isSmartPhone {
        get { return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }
    }
}
