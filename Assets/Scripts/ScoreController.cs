﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ScoreController : MonoBehaviour
{
    [SerializeField]
    private ScoreView scoreView;

    private ReactiveProperty<int> score = new ReactiveProperty<int>(0);
    public void AddScore(int panelNum) {
        score.Value += panelNum;
    }

    public int getScore {
        get { return score.Value; }
    }

    void Start()
    {
        score.Subscribe(_ =>{
            scoreView.SetScore(score.Value.ToString());
        });
    }
}
