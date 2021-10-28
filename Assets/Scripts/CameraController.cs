using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private float defaultCameraSize = 6;

    void Awake() {
        float aspect = (float)Screen.height / (float)Screen.width;
        //Debug.Log($"aspect : {aspect}");
        if (aspect >= 2) {
            mainCamera.orthographicSize = 5.9f;
        } else {
            mainCamera.orthographicSize = 5.2f;
        }
    }
}
