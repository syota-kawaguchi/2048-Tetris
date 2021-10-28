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
    private string[] itemNames;

    private IObserver<string> publisher;

    private int index;

    void Start()
    {
        index = 0;

        itemName.text = itemNames[index];

        leftButton.OnClickAsObservable().Subscribe(_ => {
            index--;
            if (index < 0) {
                index = itemNames.Length - 1;
            }
            OnChangedIndex();
        });

        rightButton.OnClickAsObservable().Subscribe(_ => {
            index++;
            if (itemNames.Length <= index) {
                index = 0;
            }
            OnChangedIndex();
        });
    }

    private void OnChangedIndex() {
        if (itemNames == null || itemNames.Length == 0) return;
        itemName.text = itemNames[index];

        publisher.OnNext(itemNames[index]);
    }

    public void Init(string[] itemNames, IObserver<string> publisher, string initValue) {
        this.itemNames = itemNames;
        this.publisher = publisher;

        itemName.text = initValue;
    }
}
