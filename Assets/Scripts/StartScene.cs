using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using AudioManager;
public class StartScene : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private Button howToPlayButton;

    void Start() {
        startButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                var seVolume = PlayerPrefs.GetFloat(SettingsController.seKey, 0.8f);
                SEManager.Instance.Play(SEPath.TAP_SOUND2, volumeRate:seVolume);
                SceneManager.LoadScene("MainScene");
            }).AddTo(this.gameObject);

        howToPlayButton.onClick.AsObservable()
            .Subscribe(_ => {
                var seVolume = PlayerPrefs.GetFloat(SettingsController.seKey, 0.8f);
                SEManager.Instance.Play(SEPath.TAP_SOUND2, volumeRate: seVolume);
                SceneManager.LoadScene("HowToPlay");
            }).AddTo(this.gameObject);
    }
}
