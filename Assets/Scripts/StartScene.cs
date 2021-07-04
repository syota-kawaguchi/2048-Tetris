using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
public class StartScene : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    void Start() {
        startButton.onClick.AsObservable()
            .Subscribe(_ => SceneManager.LoadScene("MainScene"));
    }
}
