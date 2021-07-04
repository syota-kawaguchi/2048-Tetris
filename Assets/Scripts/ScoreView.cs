using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI score;

    public void SetScore(string score) {
        this.score.text = "score : " + score;
    }
}
