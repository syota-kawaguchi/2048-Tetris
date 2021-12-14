using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AudioManager;

public class HowToPlayController : MonoBehaviour
{
    [SerializeField]
    private Page[] pages;

    private int currentPageIndex = 0;
    private float seVolume;

    void Start() {
        foreach (var page in pages) {
            page.gameObject.SetActive(false);
        }
        pages[currentPageIndex].gameObject.SetActive(true);
        seVolume = PlayerPrefs.GetFloat(SettingsController.seKey, 0.8f);
    }

    public void NextPage() {
        SEManager.Instance.Play(SEPath.TAP_SOUND2, seVolume);
        pages[currentPageIndex].gameObject.SetActive(false);
        currentPageIndex++;
        if (pages.Length <= currentPageIndex) {
            SceneManager.LoadScene("MainScene");
        }
        else {
            pages[currentPageIndex].gameObject.SetActive(true);
        }

    }

    public void BackPage() {
        SEManager.Instance.Play(SEPath.TAP_SOUND2, seVolume);
        pages[currentPageIndex].gameObject.SetActive(false);
        currentPageIndex--;
        if (currentPageIndex < 0) {
            SceneManager.LoadScene("StartScene");
        }
        else {
            pages[currentPageIndex].gameObject.SetActive(true);
        }
    }
}
