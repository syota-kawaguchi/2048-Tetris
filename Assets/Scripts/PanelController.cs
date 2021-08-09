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
    private static List<Tuple<int, int>> movePanelIndexes = new List<Tuple<int, int>>();

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

    void Start() {

        Debug.Log($"Screen : {Screen.orientation}");
        Debug.Log($"Screen size width : {Screen.width}");
        Debug.Log($"Screen size width : {Screen.height}");

        nextPanelCollection.Add(GenerateIndex());

        this.UpdateAsObservable()
            .Where(_ => !OnDenyInput())
            .Where(_ => inputManager.InputHorizontal() != 0)
            .Subscribe(_ => {
                MoveHorizontal(inputManager.InputHorizontal());
            });

        this.UpdateAsObservable()
            .Where(_ => inputManager.OnMoveDown())
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
            SyncGridWithTrans(currentPanel);
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
        SyncGridWithTrans(currentPanel);
        StartCoroutine(OrganizeGrid());
    }

    private IEnumerator OrganizeGrid() {

        onProcessing = true;

        movePanels.Add(currentPanel);

        while (movePanels.Count > 0) {
            var checkNextPanelCor = StartCoroutine(CheckNextToPanel(movePanels));
            yield return checkNextPanelCor;

            movePanels.Clear();
            movePanelIndexes.Clear();

            for (int x = 0; x < WIDTH; x++) {
                bool onModeDown = false;
                int blankX = 0;
                int blankY = 0;
                int panelIndex = 0;
                for (int y = 0; y < HEIGHT; y++) {
                    if (!onModeDown && grid[x, y] != null){
                        if (grid[x, y].getPanelIndex == panelIndex) {
                            movePanels.Add(grid[x, y]);
                        }
                        panelIndex = grid[x, y].getPanelIndex;
                        continue;
                    }

                    //空白のマスを見つけたとき
                    if (!onModeDown && grid[x, y] == null) {
                        onModeDown = true;         //次マスを見つけたとき下にずらすフラグ
                        (blankX, blankY) = (x, y);
                        continue;
                    }

                    //下にずらすパネルを実効するアニメーションとして登録
                    if (onModeDown && grid[x, y] != null) {
                        TweenAnimation.RegistMoveAnim(
                            grid[x, y].transform,
                            new Vector3(blankX, blankY, 0),
                            downMoveDuration,
                            () => { }
                        );
                        (blankX, blankY) = (x, y);
                        movePanelIndexes.Add(Tuple.Create(x, y));
                        continue;
                    }
                }
            }

            var animCoroutrine = TweenAnimation.MoveAnimRun();//TweenAnimation.MoveAnimRunShake(shakeStrength, shakeDuration);
            yield return animCoroutrine;

            foreach (var index in movePanelIndexes) {
                SyncGridWithTrans(grid[index.Item1, index.Item2]);
                var panel = grid[index.Item1, index.Item2];
                movePanels.Add(panel);
                grid[index.Item1, index.Item2] = null;
            }
        }

        if (grid[(int)instantiatePos.x, (int)instantiatePos.y]) {
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
                grid[roundX - 1, roundY].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";
                TweenAnimation.RegistMoveAnim(grid[roundX - 1, roundY].transform, purpose, addMoveDuration, () => RemoveGrid(roundX - 1, roundY), panel.transform);
            }
            if (ValidPanel(roundX + 1, roundY) && grid[roundX + 1, roundY].getPanelIndex == panel.getPanelIndex) {
                nextNums++;
                grid[roundX + 1, roundY].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";
                TweenAnimation.RegistMoveAnim(grid[roundX + 1, roundY].transform, purpose, addMoveDuration, () => RemoveGrid(roundX + 1, roundY), panel.transform);
            }
            if (ValidPanel(roundX, roundY - 1) && grid[roundX, roundY - 1].getPanelIndex == panel.getPanelIndex) {
                nextNums++;
                grid[roundX, roundY - 1].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";
                TweenAnimation.RegistMoveAnim(grid[roundX, roundY - 1].transform, purpose, addMoveDuration, () => RemoveGrid(roundX, roundY - 1), panel.transform);
            }

            panel.AddIndex(nextNums);
            if (nextNums > 0) {
                scoreController.AddScore(panel.getPanelNum);
            }
        }

        var animCoroutine = StartCoroutine(TweenAnimation.MergePanelAnim(shakeStrength, shakeDuration, mergeDuration));
        yield return animCoroutine;
    }

    private void SyncGridWithTrans(Panel panel) {
        int roundX = Mathf.RoundToInt(panel.transform.position.x);
        int roundY = Mathf.RoundToInt(panel.transform.position.y);
        grid[roundX, roundY] = panel;
    }

    private void SetGrid(Panel panel, int x, int y) {
        grid[x, y] = panel;
        grid[x, y].transform.position = new Vector3(x, y, 0);
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

    public void Restart() {
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                if(grid[i, j])Destroy(grid[i, j].gameObject);
                grid[i, j] = null;
            }
        }
        movePanels.Clear();
        movePanelIndexes.Clear();
        currentPanel = null;
        Destroy(currentPanelObj);
        nextPanelCollection.Clear();

        nextPanelCollection.Add(GenerateIndex());

        CreateNewPanel();
    }

    private IEnumerator GameOver() {
        yield return new WaitForSeconds(1.0f);

        gameOverFlag = true;
        PlayerPrefs.SetInt("Score", scoreController.getScore);
        SceneManager.LoadScene("GameOverScene");
    }
}
