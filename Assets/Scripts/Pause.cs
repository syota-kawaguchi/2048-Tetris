using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

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
                onPause = true;
                pausePanel.transform.DOScale(initScaleRatio, 0);
                pausePanel.SetActive(true);
                pausePanel.transform.DOScale(endScaleRatio, showPauseDuration);
            });

        resumeButton.onClick.AsObservable()
            .Subscribe(_ => {
                onPause = false;
                pausePanel.SetActive(false);
            });

        restartButton.OnClickAsObservable()
            .Subscribe(_ =>{
                onPause = false;
                panelController.Restart();
                pausePanel.SetActive(false);
            });

        quitButton.OnClickAsObservable()
            .Subscribe(_ =>{
                onPause = false;
                SceneManager.LoadScene("StartScene");
            });

        pausePanel.SetActive(false);
    }
}
