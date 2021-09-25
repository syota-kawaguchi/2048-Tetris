using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Grid : MonoBehaviour
{
    public const int WIDTH = 5;
    public const int HEIGHT = 7;

    private static Panel[,] grid = new Panel[WIDTH, HEIGHT];

    public static void Remove(int x, int y) {
        if (OutOfRange(x, y) || grid[x, y] == null) return;

        Destroy(grid[x, y].gameObject);
        grid[x, y] = null;
        ShowGrid.OnChangeGrid.OnNext(grid);
    }

    public static void Add(Panel panel) {
        int x = Mathf.RoundToInt(panel.transform.position.x);
        int y = Mathf.RoundToInt(panel.transform.position.y);

        if (OutOfRange(x, y)) return;

        grid[x, y] = panel;
        ShowGrid.OnChangeGrid.OnNext(grid);
    }

    public static void Sync(int x, int y) {
        if (OutOfRange(x, y)) return;

        int roundX = Mathf.RoundToInt(grid[x, y].transform.position.x);
        int roundY = Mathf.RoundToInt(grid[x, y].transform.position.y);

        if (OutOfRange(roundX, roundY)) {
            Debug.LogError("Out of range Grid Sync");
            return;
        };

        grid[roundX, roundY] = grid[x, y];
        grid[x, y] = null;
        ShowGrid.OnChangeGrid.OnNext(grid);
    }

    public static bool IsBlank(int x, int y) {
        return !OutOfRange(x, y) && grid[x, y] == null;
    }

    public static Panel GetPanel(int x, int y) {
        if (OutOfRange(x, y)) {
            Debug.Log($"GetPanel is out of range({x}, {y})");
            return null;
        }

        return grid[x, y];
    }

    public static int GetPanelIndex(int x, int y) {
        if (OutOfRange(x, y)) return -1;

        return grid[x, y].getPanelIndex;
    }

    /// <summary>
    /// 渡したx座標で最も下に位置する配置可能な場所のYを返す
    /// </summary>
    public static int GetEnablePlaceY(int x) {
        for (int y = 0; y < HEIGHT; y++) {
            if (grid[x, y] == null) return y;
        }
        return -1;
    }

    public static void Refresh() {
        var tmpGrid = new Panel[WIDTH, HEIGHT];

        for (int x = 0; x < WIDTH; x++) {
            for (int y = 0; y < HEIGHT; y++) {
                var panel = grid[x, y];
                if (panel == null) continue;

                int roundX = Mathf.RoundToInt(panel.transform.position.x);
                int roundY = Mathf.RoundToInt(panel.transform.position.y);

                if (OutOfRange(roundX, roundY)) continue;

                tmpGrid[roundX, roundY] = grid[x, y];
            }
        }

        grid = tmpGrid;
        ShowGrid.OnChangeGrid.OnNext(grid);
    }

    public static void RemoveAll() {
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                if (grid[i, j]) Destroy(grid[i, j].gameObject);
                grid[i, j] = null;
            }
        }
        ShowGrid.OnChangeGrid.OnNext(grid);
    }

    public static bool OutOfRange(int x, int y) {
        return x < 0 || WIDTH <= x || y < 0 || HEIGHT <= y;
    }

    public static bool IsEqualNextPanelIndex(Panel panel, int x, int y) {
        return ValidPanel(x, y) && GetPanelIndex(x, y) == panel.getPanelIndex && !GetPanel(x, y).mergeFlag;
    }

    public static bool ValidPanel(int x, int y) {
        return (0 <= x && x < WIDTH) && (0 <= y && y <= HEIGHT) && !IsBlank(x, y);
    }
}
