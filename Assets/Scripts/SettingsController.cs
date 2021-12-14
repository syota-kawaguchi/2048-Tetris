using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using AudioManager;

[Serializable]
public class SettingsItem {
    public int moveHorizontal;
    public int moveVertical;
    public float seVolume;
}

public enum Operation {
    Swipe = 1,
    Flick = 2,
    Tap = 3,
    DoubleTap = 4,
    FlickDown = 5 
}

public class SettingsController : SingletonMonoBehaviour<SettingsController>
{
    public SettingsItem settings;

    [SerializeField]
    private GameObject settingsPanel;

    [SerializeField]
    private Button doneButton;

    [SerializeField]
    private SettingsList horizontalSetting;
    private Operation[] horizontalMoves = new Operation[3] { Operation.Flick, Operation.Swipe, Operation.Tap };
    [SerializeField]
    private Operation defaultHorizontalMove = Operation.Flick;
    private Subject<int> horizontalMoveSubject = new Subject<int>();

    [SerializeField]
    private SettingsList verticalSetting;
    private Operation[] verticalMoves = new Operation[2] { Operation.FlickDown, Operation.DoubleTap };
    [SerializeField]
    private Operation defaultVerticalMove = Operation.FlickDown;
    private Subject<int> verticalMoveSubject = new Subject<int>();

    [SerializeField]
    private SettingsList seSetting;
    [SerializeField]
    private float defaultSeVolume = 0.8f;
    private Subject<float> seVolumeSubject = new Subject<float>();

    private static readonly string horizontalKey = "horizontal";
    private static readonly string verticalKey   = "vertical";
    public  static readonly string seKey         = "seVolume";

    // Start is called before the first frame update
    void Start()
    {
        if (!settingsPanel) settingsPanel.SetActive(true);

        settings = new SettingsItem();

        //デフォルトはキーがなければ返されるだけで、キーがない時はセットされるわけではない
        settings.moveHorizontal = SetSettingItem(horizontalKey, (int)defaultHorizontalMove);
        settings.moveVertical   = SetSettingItem(verticalKey, (int)defaultVerticalMove);
        settings.seVolume       = SetSettingItem(seKey, defaultSeVolume);

        horizontalMoveSubject.Subscribe(index => {
            settings.moveHorizontal = index;
            PlayerPrefs.SetInt(horizontalKey, index);
        });

        verticalMoveSubject.Subscribe(index => {
            settings.moveVertical = index;
            PlayerPrefs.SetInt(verticalKey, index);
        });

        seVolumeSubject.Subscribe(value => {
            settings.seVolume = value;
            PlayerPrefs.SetFloat(seKey, value);
        });

        horizontalSetting.Init(horizontalMoves, horizontalMoveSubject, (Operation)settings.moveHorizontal);

        verticalSetting.Init(verticalMoves,  verticalMoveSubject, (Operation)settings.moveVertical);

        seSetting.Init(seVolumeSubject, settings.seVolume);

        if (settingsPanel) settingsPanel.SetActive(false);
    }

    private static int SetSettingItem(string key, int defaultValue) {
        if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetInt(key);
        else {
            PlayerPrefs.SetInt(key, defaultValue);
            return defaultValue;
        }
    }

    private static float SetSettingItem(string key, float defaultValue) {
        if (PlayerPrefs.HasKey(key)) {
            return PlayerPrefs.GetFloat(key);
        }
        else {
            PlayerPrefs.SetFloat(key, defaultValue);
            return defaultValue;
        }
    }

    public void HideSettingsPanel() {
        var settings = SettingsController.Instance.settings;
        var seVolume = settings != null ? settings.seVolume : 0.8f;
        SEManager.Instance.Play(SEPath.TAP_SOUND2, volumeRate:seVolume);
        settingsPanel.SetActive(false);
    }
}