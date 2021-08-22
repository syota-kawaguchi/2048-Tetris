using System.Collections;
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

    const int WIDTH  = 5;
    const int HEIGHT = 7;

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

    public void Update() {
        Debug.Log("horizontal : " + inputManager.InputHorizontal());
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
            SyncGridWithTrans(currentPanel);
            StartCoroutine(MergePanel());
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
        StartCoroutine(MergePanel());
    }

    private IEnumerator MergePanel() {
        onProcessing = true;

        movePanels.Add(currentPanel);

        while (movePanels.Count > 0) {
            var checkNextPanelCor = CheckNextToPanel(movePanels);
            yield return StartCoroutine(checkNextPanelCor);

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
                            downMoveDuration
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
            movePanels.AddRange(AnimedPanel);
            movePanels = movePanels.Distinct().ToList();
        }

        if (grid[(int)instantiatePos.x, (int)instantiatePos.y]) {
            StartCoroutine(GameOver());
            yield break;
        }

        yield return new WaitForSeconds(nextRoopDuration);

        CreateNewPanel();

        onProcessing = false;
    }

    private List<Panel> AnimedPanel = new List<Panel>();

    private IEnumerator CheckNextToPanel(List<Panel> movePanels) {

        AnimedPanel.Clear();

        foreach (var panel in movePanels) {
            int roundX = Mathf.RoundToInt(panel.transform.position.x);
            int roundY = Mathf.RoundToInt(panel.transform.position.y);
            int nextNums = 0;

            Vector3 purpose = new Vector3(roundX, roundY, 0);

            Action<int, int> _callback = (int x, int y) => {
                nextNums++;
                grid[x, y].mergeFlag = true;
                grid[x, y].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Back";
                TweenAnimation.RegistMoveAnim(grid[x, y].transform, purpose, addMoveDuration, () => RemoveGrid(x, y), panel.transform);
            };

            if (IsEqualNextPanelIndex(panel, roundX - 1, roundY)) _callback(roundX - 1, roundY);
            if (IsEqualNextPanelIndex(panel, roundX + 1, roundY)) _callback(roundX + 1, roundY);
            if (IsEqualNextPanelIndex(panel, roundX, roundY - 1)) _callback(roundX, roundY - 1);

            panel.AddIndex(nextNums);
            if (nextNums > 0) {
                scoreController.AddScore(panel.getPanelNum);
                AnimedPanel.Add(panel);
            }
        }

        var animCoroutine = StartCoroutine(TweenAnimation.MergePanelAnim(shakeStrength, shakeDuration, mergeDuration));
        yield return animCoroutine;
    }

    private bool IsEqualNextPanelIndex(Panel panel, int x, int y) => ValidPanel(x, y) && grid[x, y].getPanelIndex == panel.getPanelIndex && !grid[x, y].mergeFlag;

    private void SyncGridWithTrans(Panel panel) {
        int roundX = Mathf.RoundToInt(panel.transform.position.x);
        int roundY = Mathf.RoundToInt(panel.transform.position.y);
        grid[roundX, roundY] = panel;
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
