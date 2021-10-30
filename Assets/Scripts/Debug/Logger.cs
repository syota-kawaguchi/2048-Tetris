using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField]
    private GameObject loggerPanel;

    [SerializeField]
    private SettingsController settingsController;

    [SerializeField]
    private Text jsonHorizontal;

    [SerializeField]
    private Text jsonVertical;

    void Start()
    {
        if (loggerPanel.activeSelf) loggerPanel.SetActive(false);
    }

    public void ShowLoggerPanel() {
        loggerPanel.SetActive(true);
    }

    public void HideLoggerPanel() {
        loggerPanel.SetActive(false);
    }

    void Update() {

        if (!loggerPanel.activeSelf) return;

        if (settingsController.settings != null) {
            jsonHorizontal.text = settingsController.settings.moveHorizontal.ToString();
            jsonVertical.text = settingsController.settings.moveVertical.ToString();
        }
        else {
            jsonHorizontal.text = "---";
            jsonVertical.text = "---";
        }
    }


}
