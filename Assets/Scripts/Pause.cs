using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;
using AudioManager;

public class Pause : MonoBehaviour
{
    public static bool onPause = false;

    [SerializeField]
    private PanelController panelController;

    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private GameObject settingsPanel;

    [Header("Pause Panle Animation")]
    [SerializeField]
    private float initScaleRatio = 0.2f;

    [SerializeField]
    private float endScaleRatio = 1.0f;

    [SerializeField]
    private float showPauseDuration = 0.2f;
    
    void Start()
    {
        pauseButton.onClick.AsObservable()
            .Subscribe(_ => {
                pausePanel.transform.DOScale(initScaleRatio, 0);
                OnTapButton(true, true);
                pausePanel.transform.DOScale(endScaleRatio, showPauseDuration);
            });

        resumeButton.onClick.AsObservable()
            .Subscribe(_ => {
                OnTapButton(false, false);
            });

        restartButton.OnClickAsObservable()
            .Subscribe(_ =>{
                OnTapButton(false, false);
                panelController.Restart();
            });

        quitButton.OnClickAsObservable()
            .Subscribe(_ =>{
                OnTapButton(false, false);
                SceneManager.LoadScene("StartScene");
            });

        settingsButton.OnClickAsObservable()
            .Subscribe(_ =>{
                OnTapButton(true, true);
                settingsPanel.SetActive(true);
            });

        pausePanel.SetActive(false);
    }

    void OnTapButton(bool _onPause, bool activeSelf) {
        onPause = _onPause;
        pausePanel.SetActive(activeSelf);
        SEManager.Instance.Play(SEPath.TAP_SOUND2);
    }
}
