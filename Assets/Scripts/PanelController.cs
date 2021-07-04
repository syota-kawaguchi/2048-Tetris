using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    const int WIDTH  = 5;
    const int HEIGHT = 7;

    //指数自動生成の下限と上限
    private int minIndex = 1;
    private int maxIndex = 8;

    private bool gameOverFlag = false;

    //TimerSpan
    private float timeSpan = 1.0f;

    //パネルが下に到達したときTrue
    private bool onProcessing = false;

    private static Panel[,] grid  = new Panel[WIDTH, HEIGHT];
    private static List<Panel> movePanels = new List<Panel>();

    [SerializeField]
    private GameObject panelPrefab;
    private Vector3 instantiatePos = new Vector3(2, 6, 0);

    private GameObject currentPanelObj;
    private Panel currentPanel;

    private ReactiveCollection<int> nextPanelCollection = new ReactiveCollection<int>();
    private ReactiveProperty<int> holdPanel = new ReactiveProperty<int>(0);

    [Header("Animation")]
    [SerializeField]
    private float addMoveDuration = 0.2f;

    [SerializeField]
    private float downMoveDuration = 0.4f;

    [SerializeField]
    private float shakeStrength = 0.7f;

    [SerializeField]
    private float shakeDuration = 0.5f;

    [SerializeField]
    private float nextRoopDuration = 0.5f;

    void Start() {
        nextPanelCollection.Add(GenerateIndex());

        this.UpdateAsObservable()
            .Where(_ => inputManager.onMoveLeft())
            .Where(_ => !OnDenyInput())
            .Subscribe(_ => MoveHorizontal(-1));

        this.UpdateAsObservable()
            .Where(_ => inputManager.onMoveRight())
            .Where(_ => !OnDenyInput())
            .Subscribe(_ => MoveHorizontal(1));

        this.UpdateAsObservable()
            .Where(_ => inputManager.onMoveDown())
            .Where(_ => !OnDenyInput())
            .Subscribe(_ => MoveDownBottom(currentPanel));

        Observable.Interval(TimeSpan.FromSeconds(timeSpan))
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
        currentPanelObj.transform.position += new Vector3(hol, 0, 0);
        if (!ValidMovement()) {
            currentPanelObj.transform.position += new Vector3(-hol, 0, 0);
        }
    }

    private void MoveDown() {
        if (!currentPanelObj) return;

        currentPanelObj.transform.position += Vector3.down;

        if (!ValidMovement()) {
            currentPanelObj.transform.position += Vector3.up;
            SetGrid(currentPanel);
            StartCoroutine(OrganizeGrid());
        }
    }

    private void MoveDownBottom(Panel panel) {
        if (!panel) return;
        int roundX = Mathf.RoundToInt(panel.transform.position.x);
        for (int roundY = 0; roundY < HEIGHT; roundY++) {
            if (grid[roundX, roundY] != null) continue;
            panel.transform.position = new Vector3(roundX, roundY, 0);
            break;
        }
        SetGrid(currentPanel);
        StartCoroutine(OrganizeGrid());
    }

    private IEnumerator OrganizeGrid() {

        onProcessing = true;

        movePanels.Add(currentPanel);

        while (movePanels.Count > 0) {
            var checkNextPanelCor = StartCoroutine(CheckNextToPanel(movePanels));
            yield return checkNextPanelCor;

            movePanels.Clear();

            for (int x = 0; x < WIDTH; x++) {
                for (int y = 1; y < HEIGHT; y++) {
                    if (grid[x, y] == null) continue;
                    for (int y2 = 0; y2 < y; y2++) {
                        if (grid[x, y2] == null) {
                            //TweenAnimation.RegistMoveAnim(
                            //    grid[x, y].transform, 
                            //    new Vector3(x, y2, 0), 
                            //    downMoveDuration, 
                            //    () => {
                            //        SwapGrid(x, y, x, y2);
                            //        movePanels.Add(grid[x, y2]);
                            //    });
                            SetGrid(grid[x, y], x, y2);
                            grid[x, y] = null;
                            grid[x, y2].transform.position = new Vector3(x, y2);
                            movePanels.Add(grid[x, y2]);
                            break;
                        }
                    }
                }
            }
            var animCoroutrine = TweenAnimation.MoveAnimRunShake(shakeStrength, shakeDuration);
            yield return animCoroutrine;
        }

        if (grid[2, 6]) {
            StartCoroutine(GameOver());
            yield break;
        }

        yield return new WaitForSeconds(nextRoopDuration);

        CreateNewPanel();

        onProcessing = false;
    }

    private IEnumerator CheckNextToPanel(List<Panel> movePanels) {
        foreach (var panel in movePanels) {
            int roundX = Mathf.RoundToInt(panel.transform.position.x);
            int roundY = Mathf.RoundToInt(panel.transform.position.y);
            int nextNums = 0;

            Vector3 purpose = new Vector3(roundX, roundY, 0);
            if (ValidPanel(roundX - 1, roundY) && grid[roundX - 1, roundY].getPanelIndex == panel.getPanelIndex) {
                nextNums++;
                TweenAnimation.RegistMoveAnim(grid[roundX - 1, roundY].transform, purpose, addMoveDuration, () => RemoveGrid(roundX - 1, roundY));
            }
            if (ValidPanel(roundX + 1, roundY) && grid[roundX + 1, roundY].getPanelIndex == panel.getPanelIndex) {
                nextNums++;
                TweenAnimation.RegistMoveAnim(grid[roundX + 1, roundY].transform, purpose, addMoveDuration, () => RemoveGrid(roundX + 1, roundY));
            }
            if (ValidPanel(roundX, roundY - 1) && grid[roundX, roundY - 1].getPanelIndex == panel.getPanelIndex) {
                nextNums++;
                TweenAnimation.RegistMoveAnim(grid[roundX, roundY - 1].transform, purpose, addMoveDuration, () => RemoveGrid(roundX, roundY - 1));
            }

            panel.AddIndex(nextNums);
            if (nextNums > 0) scoreController.AddScore(panel.getPanelNum);
        }

        var animCoroutine = StartCoroutine(TweenAnimation.MoveAnimRun());
        yield return animCoroutine;
    }

    private void SetGrid(Panel panel) {
        int roundX = Mathf.RoundToInt(panel.transform.position.x);
        int roundY = Mathf.RoundToInt(panel.transform.position.y);
        grid[roundX, roundY] = panel;
    }

    private void SetGrid(Panel panel, int x, int y) {
        grid[x, y] = panel;
        grid[x, y].transform.position = new Vector3(x, y, 0);
    }

    private void SwapGrid(int x, int y, int x2, int y2) {
        (grid[x, y], grid[x2, y2]) = (grid[x2, y2], grid[x, y]);
    }

    private void RemoveGrid(int x, int y) {
        if (OutOfRange(x, y)) return;
        Destroy(grid[x, y].gameObject);
        grid[x, y] = null;
    }

    private int GenerateIndex() {
        return UnityEngine.Random.Range(minIndex, maxIndex + 1);
    }

    private bool ValidMovement() {
        int roundX = Mathf.RoundToInt(currentPanelObj.transform.position.x);
        int roundY = Mathf.RoundToInt(currentPanelObj.transform.position.y);

        if (OutOfRange(roundX, roundY)) return false;
        if (grid[roundX, roundY] != null) return false;
        return true;
    }

    private bool ValidPanel(int x, int y) {
        return (0 <= x && x < WIDTH) && (0 <= y && y <= HEIGHT) && grid[x, y] != null;
    }

    private bool OutOfRange(int x, int y) {
        return x < 0 || WIDTH <= x || y < 0 || HEIGHT <= y;
    }

    private IEnumerator GameOver() {
        yield return new WaitForSeconds(1.0f);

        gameOverFlag = true;
        PlayerPrefs.SetInt("Score", scoreController.getScore);
        SceneManager.LoadScene("GameOverScene");
    }
}
