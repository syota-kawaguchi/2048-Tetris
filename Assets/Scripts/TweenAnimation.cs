﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TweenAnimation : MonoBehaviour
{
    private static float destroyDuration = 0.2f;

    private static List<TweenMoveAnim> MoveAnimList = new List<TweenMoveAnim>();
    public static void RegistMoveAnim(Transform target, Vector3 purpose, float duration, Action onComplete = null, Transform centerPanel = null) {
        MoveAnimList.Add(new TweenMoveAnim(target, purpose, duration, onComplete, centerPanel));
    }

    public static IEnumerator MoveAnimRun() {
        if (MoveAnimList.Count == 0) yield break;

        bool finFlag = false;
        for (int i = 0; i < MoveAnimList.Count; i++) {
            var anim = MoveAnimList[i];
            anim.target
                .DOMove(anim.purpose, anim.duration)
                .OnComplete(() => {
                    anim.onComplete?.Invoke();
                    if (i >= MoveAnimList.Count - 1) finFlag = true;
                });
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finFlag);
    }

    public static IEnumerator MoveThenDestroyText() {
        if (MoveAnimList.Count == 0) yield break;

        bool finFlag = false;
        for (int i = 0; i < MoveAnimList.Count; i++) {
            var anim = MoveAnimList[i];
            anim.target
                .DOMove(anim.purpose, anim.duration)
                .OnComplete(() => {
                    anim.onComplete?.Invoke();
                    if (i >= MoveAnimList.Count - 1) finFlag = true;
                });
            DOVirtual.DelayedCall(
                destroyDuration,
                () => DestroyChildren(anim.target)
            );
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finFlag);
    }

    public static IEnumerator MergePanelAnim(float strength, float shakeTime, float mergeDuration) {
        if (MoveAnimList.Count == 0) yield break;

        bool finMergeFlag = false;
        for (int i = 0; i < MoveAnimList.Count; i++) {
            var anim = MoveAnimList[i];
            anim.target
                .DOMove(anim.purpose, anim.duration)
                .OnComplete(() => {
                    anim.onComplete?.Invoke();
                    if (anim.centerPanel) {
                        anim.centerPanel
                        .DOShakeScale(strength, shakeTime)
                        .OnComplete(() =>{
                            finMergeFlag = true;
                        });
                    }
                    if (i >= MoveAnimList.Count - 1) finMergeFlag = true;
                });
            DOVirtual.DelayedCall(
                destroyDuration,
                () => DestroyChildren(anim.target)
            );
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finMergeFlag);
        yield return new WaitForSeconds(mergeDuration);
    }

    public static IEnumerator MoveAnimRunShake(float strength, float shakeTime) {
        if (MoveAnimList.Count == 0) yield break;

        bool finFlag = false;
        for (int i = 0; i < MoveAnimList.Count; i++) {
            var anim = MoveAnimList[i];
            anim.target.DOMove(anim.purpose, anim.duration)
                       .SetDelay(anim.duration);
            anim.target.DOShakeScale(strength, shakeTime)
                       .OnComplete(() => {
                           anim.onComplete?.Invoke();
                           if (i >= MoveAnimList.Count - 1) {
                               finFlag = true;
                               anim.target.DOKill();
                           }
                       });
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finFlag);
    }

    private static void DestroyChildren(Transform parent) {
        foreach (Transform child in parent) {
            Destroy(child);
        }
    }
}

public struct TweenMoveAnim{
    public Transform target;
    public Vector3 purpose;
    public float duration;
    public Action onComplete;
    public Transform centerPanel;

    public TweenMoveAnim(Transform target, Vector3 purpose, float duration, Action onComplete, Transform centerPanel = null) {
        this.target = target;
        this.purpose = purpose;
        this.duration = duration;
        this.onComplete = onComplete;
        this.centerPanel = centerPanel;
    }
}
