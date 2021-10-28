using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputChecker : MonoBehaviour
{
    [SerializeField]
    private Text flickDirection;

    [SerializeField]
    private Text flickValue;

    [SerializeField]
    private Text swipeDirection;

    [SerializeField]
    private Text swipeValue;

    [SerializeField]
    private Text doubleTap;

    // Update is called once per frame
    void Update()
    {
        flickDirection.text = ScreenInput.Instance.getFlickDirection.ToString();
        swipeDirection.text = ScreenInput.Instance.getSwipeDirection.ToString();
        swipeValue.text = ScreenInput.Instance.getSwipeValueVec.ToString();
        doubleTap.text  = ScreenInput.Instance.DoubleTap().ToString();
    }
}
