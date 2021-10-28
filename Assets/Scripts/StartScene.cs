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

    void Start() {
        startButton.onClick.AsObservable()
            .Subscribe(_ =>
            {
                SEManager.Instance.Play(SEPath.TAP_SOUND2);
                SceneManager.LoadScene("MainScene");
            });
    }
}
