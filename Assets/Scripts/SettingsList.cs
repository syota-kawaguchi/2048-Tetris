using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class SettingsList : MonoBehaviour
{
    [SerializeField]
    private Button leftButton;

    [SerializeField]
    private Button rightButton;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private Operation[] items;

    private IObserver<int> publisher;

    private int index;

    void Start()
    {
        index = 0;

        itemName.text = items[index].ToString();

        leftButton.OnClickAsObservable().Subscribe(_ => {
            index--;
            if (index < 0) {
                index = items.Length - 1;
            }
            OnChangedIndex();
        });

        rightButton.OnClickAsObservable().Subscribe(_ => {
            index++;
            if (items.Length <= index) {
                index = 0;
            }
            OnChangedIndex();
        });
    }

    private void OnChangedIndex() {
        if (items == null || items.Length == 0) return;
        itemName.text = items[index].ToString();

        publisher.OnNext((int)items[index]);
    }

    public void Init(Operation[] items, IObserver<int> publisher, Operation item) {
        this.items = items;
        this.publisher = publisher;

        itemName.text = item.ToString();
    }
}
