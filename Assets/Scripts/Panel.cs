using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UniRx;

public class Panel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI panelNum;
    [SerializeField]
    private SpriteRenderer panel;

    private ReactiveProperty<int> index = new ReactiveProperty<int>(1);

    public void Init(int initIndex) {
        index.Value = initIndex;

        index.Subscribe(num => {
            panelNum.text = getPanelNum.ToString();
        }).AddTo(gameObject);

        panel.color = PanelColors.GetColor(index.Value);
    }

    public int getPanelIndex {
        get { return index.Value; }
    }

    public void DecSortingLayer() {
        panel.sortingOrder--;
    }

    //指数計算
    public void AddIndex(int num) {
        index.Value += num;
        panel.color = PanelColors.GetColor(index.Value);
    }

    //2の累乗を表示
    public int getPanelNum {
        get { return (int)Math.Pow(2, index.Value); }
    }
}
