﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PanelController : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private ScoreController scoreController;

    [SerializeField]
    private NextPanelView nextPanelView;

    [SerializeField]
    private HoldView holdView;

    [SerializeField]
    private Button holdButton;

    //指数自動生成の下限と上限
    private int minIndex = 1;
    private int maxIndex = 8;

    private bool gameOverFlag = false;

    //move down TimerSpan
    [SerializeField]
    private float moveDownTimeSpan = 1.0f;

     [SerializeField]
    private float inputTimeSpan = 0.3f;

    //パネルが下に到達したときTrue
    private bool onProcessing = false;

    [SerializeField]
    private GameObject panelPrefab;
    private readonly Vector3 instantiatePos = new Vector3(2, 6, 0);

    private GameObject currentPanelObj;
    private Panel currentPanel;

    private ReactiveCollection<int> nextPanelCollection = new ReactiveCollection<int>();
    private ReactiveProperty<int> holdPanel = new ReactiveProperty<int>(0);

    [Header("Animation")]
    [SerializeField]
    private float addMoveDuration = 0.15f;

    [SerializeField]
    private float downMoveDuration = 0.3f;

    [SerializeField]
    private float shakeStrength = 0.7f;

    [SerializeField]
    private float shakeDuration = 0.5f;

    [SerializeField]
    private float nextRoopDuration = 0.5f;

    [SerializeField]
    private float mergeDuration = 0.5f; //パネルが合体してから次の処理までの間時間

    [SerializeField]
    private float gameOverSceneDuration = 1.0f;

    void Start() {

        Debug.Log($"Screen : {Screen.orientation}");
        Debug.Log($"Screen size width : {Screen.width}");
        Debug.Log($"Screen size width : {Screen.height}");

        nextPanelCollection.Add(GenerateIndex());

        this.UpdateAsObservable()
            .Where(_ => inputManager.InputHorizontal() != 0)
            .Where(_ => !OnDenyInput())
            .ThrottleFirst(TimeSpan.FromSeconds(inputTimeSpan))
            .Subscribe(_ => MoveHorizontal(inputManager.InputHorizontal()));

        this.UpdateAsObservable()
            .Where(_ => inputManager.OnMoveDown())
            .Where(_ => !OnDenyInput())
            .Subscribe(_ => MoveDownBottom(currentPanel));

        Observable.Interval(TimeSpan.FromSeconds(moveDownTimeSpan))
            .Where(_ => !OnDenyInput())
            .Subscribe(x => {
                MoveDown();
            })
            .AddTo(gameObject);

        holdButton.onClick.AsObservable()
            .Subscribe(_ => OnClickHoldButton());

        nextPanelCollection.ObserveReplace()
            .Subscribe((CollectionReplaceEvent<int> i) => nextPanelView.SetNextPanel(i.NewValue));

        CreateNewPanel();
    }

    private bool OnDenyInput() {
        return !currentPanelObj || onProcessing || Pause.onPause;
    }

    private void OnClickHoldButton() { 
    }

    private void CreateNewPanel() {
        currentPanelObj = Instantiate(panelPrefab, instantiatePos, Quaternion.identity);
        currentPanel = currentPanelObj.GetComponent<Panel>();
        currentPanel.Init(nextPanelCollection[0]);
        nextPanelCollection[0] = GenerateIndex();
    }

    private void MoveHorizontal(float hol) {
        int value = hol > 0 ? 1 : 0 > hol? -1 : 0; //0より大きい → １、0より小さい → -1、0 → 0
        currentPanelObj.transform.position += new Vector3(value, 0, 0);
        if (!ValidMovement()) {
            currentPanelObj.transform.position += new Vector3(-value, 0, 0);
        }
    }

    private void MoveDown() {
        if (!currentPanelObj) return;

        currentPanelObj.transform.position += Vector3.down;

        if (!ValidMovement()) {
            currentPanelObj.transform.position += Vector3.up;
            Grid.Add(currentPanel);
            StartCoroutine(FallAndMerge(currentPanel));
        }
    }

    private void MoveDownBottom(Panel panel) {
        if (!panel) return;
        int roundX = Mathf.RoundToInt(panel.transform.position.x);
        for (int roundY = 0; roundY < Grid.HEIGHT; roundY++) {
            if (!Grid.IsBlank(roundX, roundY)) continue;
            panel.transform.position = new Vector3(roundX, roundY, 0);
            break;
        }
        Grid.Add(panel);
        StartCoroutine(FallAndMerge(panel));
    }

    // TODO: merge後のPanelの値が隣り合う(又は上下)Panelの値と等しいときMergeが走らない  

    private IEnumerator FallAndMerge(Panel panel) {

        onProcessing = true;

        var modefiedPanels = new List<Panel>() { panel };

        while (true) {
            var mergeCol = MergePanel(modefiedPanels);
            yield return StartCoroutine(mergeCol);
            Grid.Refresh();

            var fallCol = FallPanel();
            yield return StartCoroutine(fallCol); ;
            Grid.Refresh();

            modefiedPanels = fallCol.Current as List<Panel>;
            modefiedPanels.AddRange(mergeCol.Current as List<Panel>);
            modefiedPanels = modefiedPanels.Distinct().ToList();

            if (modefiedPanels.Count == 0) break;
        }

        yield return new WaitForSeconds(nextRoopDuration);

        if (IsGameOver()) {
            yield return new WaitForSeconds(gameOverSceneDuration);

            gameOverFlag = true;
            PlayerPrefs.SetInt("Score", scoreController.getScore);
            SceneManager.LoadScene("GameOverScene");
            yield break;
        }

        CreateNewPanel();

        onProcessing = false;
    }

    private IEnumerator FallPanel() {
        var fallablePanelList = new List<Panel>();

        //下に詰めるパネルの探索＆落下アニメーションの登録
        for (int x = 0; x < Grid.WIDTH; x++) {
            var enablePlaceY = Grid.GetEnablePlaceY(x);

            if (enablePlaceY < 0) continue;

            for (int y = enablePlaceY+1; y < Grid.HEIGHT; y++) {
                var panel = Grid.GetPanel(x, y);

                if (!panel) continue;

                fallablePanelList.Add(panel);
                TweenAnimation.RegistMoveAnim(panel.transform, new Vector3(x, enablePlaceY, 0), downMoveDuration);

                enablePlaceY++; //次の空白の場所へと更新する
            }
        }

        //アニメーションの実行
        var animCoroutrine = TweenAnimation.MoveAnimRun();
        yield return StartCoroutine(animCoroutrine);
        yield return fallablePanelList;
    }

    //Want: Actionをもっと効率的に設定したい

    private IEnumerator MergePanel(List<Panel> fellPanel) {

        var modifiedPanels = new List<Panel>();

        foreach (var panel in fellPanel) {
            //Debug
            if (!panel) {
                Debug.LogError("panel is null");
                continue;
            }

            int roundX = Mathf.RoundToInt(panel.transform.position.x);
            int roundY = Mathf.RoundToInt(panel.transform.position.y);
            var purpose = panel.transform.position;

            Action<int, int> SetMergePanelAnim = (int x, int y) => {
                var _panel = Grid.GetPanel(x, y);

                _panel.mergeFlag = true;
                _panel.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";

                panel.AddIndex(1);
                TweenAnimation.RegistMoveAnim(
                    _panel.transform,
                    purpose,
                    downMoveDuration,
                    () => {
                        Grid.Remove(x, y);
                        panel.UpdateIndex();
                        panel.mergeFlag = false;
                        scoreController.AddScore(panel.getPanelNum);
                    }
                );
            };
            //right
            for (int x = roundX + 1; x < Grid.WIDTH; x++) {
                var _panel = Grid.GetPanel(x, roundY);
                if (IsQuitSearchMergePanel(panel, _panel)) break;

                SetMergePanelAnim(x, roundY);
            }
            //left
            for (int x = roundX - 1; 0 <= x; x--) {
                var _panel = Grid.GetPanel(x, roundY);
                if (IsQuitSearchMergePanel(panel, _panel)) break;

                SetMergePanelAnim(x, roundY);
            }
            //Up
            for (int y = roundY + 1; y < Grid.HEIGHT; y++) {
                var _panel = Grid.GetPanel(roundX, y);
                if (IsQuitSearchMergePanel(panel, _panel)) break;

                SetMergePanelAnim(roundX, y);
            }
            //Down
            if (roundY - 1 < 0) continue;
            var underPanel = Grid.GetPanel(roundX, roundY - 1);
            if (underPanel && underPanel.getPanelIndex == panel.getPanelIndex) {
                underPanel.mergeFlag = true;
                underPanel.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";

                panel.AddIndex(1);
                TweenAnimation.RegistMoveAnim(
                    underPanel.transform, 
                    purpose, 
                    downMoveDuration, 
                    () => {
                        Grid.Remove(roundX, roundY - 1);
                        panel.UpdateIndex();
                        panel.mergeFlag = false;
                        scoreController.AddScore(panel.getPanelNum);
                    }
                );
            }

            if (panel.GetDeltaIndex > 0) {
                panel.mergeFlag = true;
                modifiedPanels.Add(panel);
            }
        }

        var animCoroutrine = TweenAnimation.MoveAnimRun();
        yield return StartCoroutine(animCoroutrine);
        yield return modifiedPanels;
    }

    private bool IsQuitSearchMergePanel(Panel p1, Panel p2) {//p1 <- p2
        if (!p1 || !p2) return true;
        if (p1.mergeFlag || p2.mergeFlag) return true;
        if (p1.getPanelIndex != p2.getPanelIndex) return true;
        return false;
    }

    private int GenerateIndex() {
        return UnityEngine.Random.Range(minIndex, maxIndex + 1);
    }

    private bool ValidMovement() {
        int roundX = Mathf.RoundToInt(currentPanelObj.transform.position.x);
        int roundY = Mathf.RoundToInt(currentPanelObj.transform.position.y);

        if (Grid.OutOfRange(roundX, roundY)) return false;
        if (!Grid.IsBlank(roundX, roundY)) return false;
        return true;
    }

    public void Restart() {
        Grid.Delete();
        currentPanel = null;
        Destroy(currentPanelObj);
        nextPanelCollection.Clear();

        nextPanelCollection.Add(GenerateIndex());

        CreateNewPanel();
    }

    private bool IsGameOver() {
        return Grid.GetPanel((int)instantiatePos.x, (int)instantiatePos.y) != null;
    }
}
