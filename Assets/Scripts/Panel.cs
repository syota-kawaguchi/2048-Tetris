using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UniRx;

public class Panel : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI panelNum;
    [SerializeField]
    private SpriteRenderer panel;

    public bool mergeFlag = false;

    private int deltaIndex = 0;

    private int currentLane = 3;

    private ReactiveProperty<int> index = new ReactiveProperty<int>(1);

    public void Init(int initIndex) {
        index.Value = initIndex;

        index.Subscribe(num => {
            panelNum.text = getPanelNum.ToString();
        }).AddTo(gameObject);

        Color color = PanelColors.GetColor(index.Value);
        panelNum.color = PanelColors.GetTextColor(index.Value);
        panel.color = new Color(color.r, color.g, color.b);

        currentLane = (int)Mathf.Floor(transform.position.x) + 1;
    }

    private void Update() {
        if (currentLane != (int)Mathf.Floor(transform.position.x) + 1) {
            currentLane = (int)Mathf.Floor(transform.position.x) + 1;
        }
    }

    public int getPanelIndex {
        get { return index.Value; }
    }

    public void DecSortingLayer() {
        panel.sortingOrder--;
    }

    //指数計算
    public void AddIndex(int value) {
        deltaIndex += value;
    }

    public int GetDeltaIndex {
        get { return deltaIndex; }
    }

    //2の累乗を表示
    public int getPanelNum {
        get { return (int)Math.Pow(2, index.Value); }
    }

    public void UpdateIndex() {
        if (deltaIndex == 0) return;
        index.Value += deltaIndex;
        deltaIndex = 0;
        panel.color = PanelColors.GetColor(index.Value);
        panelNum.color = PanelColors.GetTextColor(index.Value);
    }

    public int getCurrentLane {
        get { return currentLane; }
    }
}
