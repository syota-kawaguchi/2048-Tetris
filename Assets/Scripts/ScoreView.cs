using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI score;

    [SerializeField]
    private TextMeshProUGUI deltaScore;

    [SerializeField]
    private float fadeoutDuration = 1.0f;

    [SerializeField]
    private Transform animeStart;

    [SerializeField]
    private Transform animeEnd;

    private void Start() {
        deltaScore.gameObject.SetActive(false);
    }

    public void SetScore(string score) {
        this.score.text = "score : " + score;
    }

    public void AddScoreAnimation(string score) {
        deltaScore.gameObject.SetActive(true);
        deltaScore.text = "+" + score;
        deltaScore.transform.position = animeStart.position;
        var tmpColor = deltaScore.color;
        tmpColor.a = 1.0f;
        deltaScore.color = tmpColor;

        DOTween.ToAlpha(
            () => deltaScore.color,
            color => deltaScore.color = color,
            0f, //目標値
            fadeoutDuration  //所要時間
            ).OnComplete(() => deltaScore.gameObject.SetActive(false));

        deltaScore.transform.DOMove(animeEnd.position,fadeoutDuration);
    }
}
