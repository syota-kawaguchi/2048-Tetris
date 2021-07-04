using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NextPanelView : MonoBehaviour
{
    [SerializeField]
    private GameObject nextPanel;

    [SerializeField]
    private TextMeshProUGUI panelNum;

    public void SetNextPanel(int panelNum) {
        this.panelNum.text = Mathf.Pow(2, panelNum).ToString();
    }
}
