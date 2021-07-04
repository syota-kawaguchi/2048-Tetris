using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoldView : MonoBehaviour {
    [SerializeField]
    private GameObject holdPanel;

    [SerializeField]
    private TextMeshProUGUI holdNum;

    public void SetHoldNum(int panelNum) {
        holdNum.text = panelNum.ToString();
    }
}
