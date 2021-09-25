using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float interval;

    private Action intervalAction;

    private Action<bool> onPause;

    private float currentTime = 0;

    private void Update() {
        currentTime += Time.deltaTime;
        if (currentTime >= interval) {
            intervalAction?.Invoke();
            currentTime = 0;
        }
    }

    public void Set(float interval, Action action, Action<bool> onPause) {
        this.interval  = interval;
        intervalAction = action;
        this.onPause = onPause;
    }

    public void Restart() {
        currentTime = 0;
    }
}
