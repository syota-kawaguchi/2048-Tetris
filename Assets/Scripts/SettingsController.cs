using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

[Serializable]
public class SettingsItem {
    public int moveHorizontal;
    public int moveVertical;
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

    private static readonly string horizontalKey = "horizontal";
    private static readonly string verticalKey   = "vertical"; 

    // Start is called before the first frame update
    void Start()
    {
        if (!settingsPanel) settingsPanel.SetActive(true);

        settings = new SettingsItem();

        settings.moveHorizontal = PlayerPrefs.GetInt(horizontalKey, (int)defaultHorizontalMove);
        settings.moveVertical   = PlayerPrefs.GetInt(verticalKey, (int)defaultVerticalMove);

        if (settings.moveHorizontal == 0) {
            settings.moveHorizontal = (int)defaultHorizontalMove;
            PlayerPrefs.SetInt(horizontalKey, (int)defaultHorizontalMove);
        }

        if (settings.moveVertical == 0) {
            settings.moveVertical = (int)defaultVerticalMove;
            PlayerPrefs.SetInt(verticalKey, (int)defaultVerticalMove);
        }

        horizontalMoveSubject.Subscribe(index => {
            settings.moveHorizontal = index;
            PlayerPrefs.SetInt(horizontalKey, index);
        });

        verticalMoveSubject.Subscribe(index => {
            settings.moveVertical = index;
            PlayerPrefs.SetInt(verticalKey, index);
        });

        horizontalSetting.Init(horizontalMoves, horizontalMoveSubject, (Operation)settings.moveHorizontal);

        verticalSetting.Init(verticalMoves,  verticalMoveSubject, (Operation)settings.moveVertical);

        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void HideSettingsPanel() {
        settingsPanel.SetActive(false);
    }
}