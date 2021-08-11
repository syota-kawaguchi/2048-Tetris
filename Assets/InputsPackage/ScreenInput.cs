﻿using UnityEngine;
using System;

// フリックの方向
public enum Direction {
    NONE,
    TAP,
    UP,
    RIGHT,
    DOWN,
    LEFT,
}

public class ScreenInput : SingletonMonoBehaviour<ScreenInput>
{
    // フリック最小移動距離
    [SerializeField]
    private Vector2 FlickMinRange = new Vector2(30.0f, 30.0f);
    // スワイプ最小移動距離
    [SerializeField]
    private Vector2 SwipeMinRange = new Vector2(50.0f, 50.0f);
    // TAPをNONEに戻すまでのカウント
    [SerializeField]
    private int NoneCountMax = 2;
    private int NoneCountNow = 0;
    // スワイプ入力距離
    private Vector2 SwipeRange;
    // 入力方向記録用
    private Vector2 inputStart;
    private Vector2 inputMove;
    private Vector2 inputEnd;

    private Direction flickDirection = Direction.NONE;
    private Direction swipeDirection = Direction.NONE;


    // Update is called once per frame
    void Update() {
        GetInputVector();
        //Debug.Log($"current flick Direction : {flickDirection}");
        //Debug.Log($"current swipe Direction : {swipeDirection}");
    }

    // 入力の取得
    private void GetInputVector() {
        // Unity上での操作取得
        if (Application.isEditor) {
            if (Input.GetMouseButtonDown(0)) {
                inputStart = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0)) {
                inputMove = Input.mousePosition;
                SwipeCalc();
            }
            else if (Input.GetMouseButtonUp(0)) {
                inputEnd = Input.mousePosition;
                FlickCalc();
            }
            else if (flickDirection != Direction.NONE || swipeDirection != Direction.NONE) {
                ResetParameter();
            }
        }
        // 端末上での操作取得
        else {
            if (Input.touchCount > 0) {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began) {
                    inputStart = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved) {
                    inputMove = Input.mousePosition;
                    SwipeCalc();
                }
                else if (touch.phase == TouchPhase.Ended) {
                    inputEnd = touch.position;
                    FlickCalc();
                }
            }
            else if (flickDirection != Direction.NONE || swipeDirection != Direction.NONE) {
                ResetParameter();
            }
        }
    }

    // 入力内容からフリック方向を計算
    private void FlickCalc() {
        Vector2 _work = new Vector2((new Vector3(inputEnd.x, 0, 0) - new Vector3(inputStart.x, 0, 0)).magnitude, (new Vector3(0, inputEnd.y, 0) - new Vector3(0, inputStart.y, 0)).magnitude);

        if (_work.x <= FlickMinRange.x && _work.y <= FlickMinRange.y) {
            flickDirection = Direction.TAP;
        }
        else if (_work.x > _work.y) {
            float _x = Mathf.Sign(inputEnd.x - inputStart.x);
            if (_x > 0) flickDirection = Direction.RIGHT;
            else if (_x < 0) flickDirection = Direction.LEFT;
        }
        else {
            float _y = Mathf.Sign(inputEnd.y - inputStart.y);
            if (_y > 0) flickDirection = Direction.UP;
            else if (_y < 0) flickDirection = Direction.DOWN;
        }
    }

    // 入力内容からスワイプ方向を計算
    private void SwipeCalc() {
        SwipeRange = new Vector2((new Vector3(inputMove.x, 0, 0) - new Vector3(inputStart.x, 0, 0)).magnitude, (new Vector3(0, inputMove.y, 0) - new Vector3(0, inputStart.y, 0)).magnitude);

        if (SwipeRange.x <= SwipeMinRange.x && SwipeRange.y <= SwipeMinRange.y) {
            swipeDirection = Direction.TAP;
        }
        else if (SwipeRange.x > SwipeRange.y) {
            float _x = Mathf.Sign(inputMove.x - inputStart.x);
            if (_x > 0) swipeDirection = Direction.RIGHT;
            else if (_x < 0) swipeDirection = Direction.LEFT;
        }
        else {
            float _y = Mathf.Sign(inputMove.y - inputStart.y);
            if (_y > 0) swipeDirection = Direction.UP;
            else if (_y < 0) swipeDirection = Direction.DOWN;
        }
    }

    // NONEにリセット
    private void ResetParameter() {
        NoneCountNow++;
        if (NoneCountNow >= NoneCountMax) {
            NoneCountNow = 0;
            flickDirection = Direction.NONE;
            swipeDirection = Direction.NONE;
            SwipeRange = new Vector2(0, 0);
        }
    }

    // フリック方向の取得
    public Direction getFlickDirection {
        get { return flickDirection; }
    }

    // スワイプ方向の取得
    public Direction getSwipeDirection {
        get { return swipeDirection; }
    }

    // スワイプ量の取得
    public float getSwipeValue {
        get {
            if (SwipeRange.x > SwipeRange.y) return SwipeRange.x;
            else return SwipeRange.y;
        }
    }

    //スワイプ量の取得
    public float getSwipeValueX {
        get { return SwipeRange.x; }
    }

    // スワイプ量の取得
    public Vector2 getSwipeValueVec {
        get {
            if (swipeDirection != Direction.NONE) return new Vector2(inputMove.x - inputStart.x, inputMove.y - inputStart.y);
            else return new Vector2(0, 0);
        }
    }
}
