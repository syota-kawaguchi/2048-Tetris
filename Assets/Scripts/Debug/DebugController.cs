using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;

public class DebugController : MonoBehaviour
{
    private enum Phase {
        SET,
        PLAY,
        END
    }

    private ReactiveProperty<Phase> phase = new ReactiveProperty<Phase>(Phase.SET);

    [SerializeField]
    private PanelController panelController;

    [SerializeField]
    private GameObject panelPrefab;

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button addButton;

    [SerializeField]
    private TMP_InputField inputPosX;

    [SerializeField]
    private TMP_InputField inputPosY;

    [SerializeField]
    private Dropdown indexDropdown;

    [SerializeField]
    private Toggle isFallToggle;

    private Panel fallPanel;

    private void Awake() {
        panelController.OnModeDebug = true;
    }

    void Start()
    {
        phase.Subscribe(p => {
            switch (p) {
                case Phase.SET:
                    playButton.gameObject.SetActive(true);
                    addButton.gameObject.SetActive(true);
                    restartButton.gameObject.SetActive(false);
                    break;
                case Phase.PLAY:
                    StartCoroutine(panelController.FallAndMerge(fallPanel));
                    phase.Value = Phase.END;
                    break;
                case Phase.END:
                    playButton.gameObject.SetActive(false);
                    addButton.gameObject.SetActive(false);
                    restartButton.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        });

        playButton.OnClickAsObservable().Subscribe(_ => {
            Debug.Log("playButtonClicked");
            if (!fallPanel) {
                Debug.Log("Set fall Panel");
            }
            phase.Value = Phase.PLAY;
        });

        restartButton.OnClickAsObservable().Subscribe(_ => {
            Grid.RemoveAll();
            phase.Value = Phase.SET;
        });

        addButton.OnClickAsObservable().Subscribe(_ => {
            var index = indexDropdown.value + 1;

            if (inputPosX.text == "" || inputPosY.text == "") {
                Debug.LogError("input position");
                return;
            }
            var posX = int.Parse(inputPosX.text);
            var posY = int.Parse(inputPosY.text);

            SetPanel(posX, posY, index, isFallToggle.isOn);
        });
    }

    public void SetPanel(int x, int y, int index, bool isfall = false) {

        if (Grid.OutOfRange(x, y)) return;

        var instantiatePos = new Vector3(x, y, 0);
        var panel = Instantiate(panelPrefab, instantiatePos, Quaternion.identity).GetComponent<Panel>();
        panel.Init(index);
        Grid.Add(panel);

        if (isfall) fallPanel = panel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
