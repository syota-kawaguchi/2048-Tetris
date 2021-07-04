using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.SceneManagement;
public class GameOverScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreValueText;

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button backToHomeButton;

    void Start()
    {
        scoreValueText.text = PlayerPrefs.GetInt("Score").ToString();

        retryButton.onClick.AsObservable()
            .Subscribe(_ => SceneManager.LoadScene("MainScene"));

        backToHomeButton.onClick.AsObservable()
            .Subscribe(_ => SceneManager.LoadScene("StartScene"));
    }
}
