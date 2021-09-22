using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class ShowGrid : MonoBehaviour
{
    [SerializeField]
    private GameObject[] gridX0;

    [SerializeField]
    private GameObject[] gridX1;

    [SerializeField]
    private GameObject[] gridX2;

    [SerializeField]
    private GameObject[] gridX3;

    [SerializeField]
    private GameObject[] gridX4;

    private GameObject[][] grid = new GameObject[5][];

    private TextMeshProUGUI[,] gridTexts;

    public static Subject<Panel[,]> OnChangeGrid = new Subject<Panel[,]>();

    void Start()
    {
        grid[0] = gridX0;
        grid[1] = gridX1;
        grid[2] = gridX2;
        grid[3] = gridX3;
        grid[4] = gridX4;

        gridTexts = new TextMeshProUGUI[grid.Length, grid[0].Length];
        for (int x = 0; x < grid.Length; x++) {
            for (int y = 0; y < grid.Length; y++) {
                gridTexts[x, y] = grid[x][y].GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        OnChangeGrid.Subscribe(panels =>
        {
            for (int x = 0; x < grid.Length; x++) {
                for (int y = 0; y < grid.Length; y++) {
                    if (panels[x, y]) {
                        var index = panels[x, y].getPanelIndex;
                        gridTexts[x, y].text = Mathf.Pow(2, index).ToString();
                    }
                    else {
                        gridTexts[x, y].text = "-";
                    }
                }
            }
        });
    }
}
