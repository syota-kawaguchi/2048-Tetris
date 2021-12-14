using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AudioManager;
using UniRx;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
public class GameOverScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreValueText;

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button backToHomeButton;

    private InterstitialAd interstitial;
    private readonly string adUnitId = "ca-app-pub-5728922507498585/9828628293";

    void Start()
    {
        scoreValueText.text = PlayerPrefs.GetInt("Score").ToString();
        var seVolume = PlayerPrefs.GetFloat(SettingsController.seKey, 0.8f);

        interstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);

        StartCoroutine(ShowAd());

        retryButton.onClick.AsObservable()
            .Subscribe(_ => {
                SEManager.Instance.Play(SEPath.TAP_SOUND2, seVolume);
                SceneManager.LoadScene("MainScene");
            })
            .AddTo(this.gameObject);

        backToHomeButton.onClick.AsObservable()
            .Subscribe(_ => {
                SEManager.Instance.Play(SEPath.TAP_SOUND2, seVolume);
                SceneManager.LoadScene("StartScene");
            })
            .AddTo(this.gameObject);
    }

    private IEnumerator ShowAd() {
        yield return new WaitUntil(() => interstitial.IsLoaded());

        interstitial.Show();
    }
}
