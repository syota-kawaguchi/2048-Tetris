using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[Serializable]
public class SettingsItem {
    public readonly string horizontalKey = "horizontal";
    public readonly string verticalkey = "vertical";

    public string moveHorizontal;
    public string moveVertical;
}

public class SettingsController : SingletonMonoBehaviour<SettingsController>
{
    public const string SWIPE = "Swipe";
    public const string FLICK = "Flick";
    public const string TAP   = "Tap";
    public const string DOUBLETAP = "DoubleTap";
    public const string FLICKDOWN = "FlickDown";

    public SettingsItem settings;

    [SerializeField]
    private GameObject settingsPanel;

    [SerializeField]
    private Button doneButton;

    [SerializeField]
    private SettingsList horizontalSetting;
    private string[] horizontalMoves = new string[3] { FLICK, SWIPE, TAP };
    [SerializeField]
    private string defaultHorizontalMove = FLICK;
    private Subject<string> horizontalMoveSubject = new Subject<string>();

    [SerializeField]
    private SettingsList verticalSetting;
    private string[] verticalMoves = new string[2] { FLICKDOWN, DOUBLETAP };
    [SerializeField]
    private string defaultVerticalMove = FLICKDOWN;
    private Subject<string> verticalMoveSubject = new Subject<string>();

    // Start is called before the first frame update
    void Start()
    {
        if (!settingsPanel) settingsPanel.SetActive(true);

        settings = new SettingsItem();

        settings.moveHorizontal = PlayerPrefs.GetString(settings.horizontalKey, defaultHorizontalMove);
        settings.moveVertical = PlayerPrefs.GetString(settings.verticalkey, defaultVerticalMove);

        if (settings.moveHorizontal == "") {
            settings.moveHorizontal = defaultHorizontalMove;
            PlayerPrefs.SetString(settings.horizontalKey, defaultHorizontalMove);
        }

        if (settings.moveVertical == "") {
            settings.moveVertical = defaultVerticalMove;
            PlayerPrefs.SetString(settings.verticalkey, defaultVerticalMove);
        }

        horizontalMoveSubject.Subscribe(s => {
            settings.moveHorizontal = s;
            PlayerPrefs.SetString(settings.horizontalKey, s);
        });

        verticalMoveSubject.Subscribe(s => {
            settings.moveVertical = s;
            PlayerPrefs.SetString(settings.verticalkey, s);
        });

        horizontalSetting.Init(horizontalMoves, horizontalMoveSubject, settings.moveHorizontal);

        verticalSetting.Init(verticalMoves,  verticalMoveSubject, settings.moveVertical);

        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void HideSettingsPanel() {
        settingsPanel.SetActive(false);
    }
}