using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class TweenAnimation : MonoBehaviour
{
    private static List<TweenMoveAnim> MoveAnimList = new List<TweenMoveAnim>();
    public static void RegistMoveAnim(Transform target, Vector3 purpose, float duration, Action onComplete) {
        MoveAnimList.Add(new TweenMoveAnim(target, purpose, duration, onComplete));
    }

    public static IEnumerator MoveAnimRun() {
        if (MoveAnimList.Count == 0) yield break;

        bool finFlag = false;
        for (int i = 0; i < MoveAnimList.Count; i++) {
            var anim = MoveAnimList[i];
            anim.target
                .DOMove(anim.purpose, anim.duration)
                .OnComplete(() => {
                    anim.onComplete();
                    if (i >= MoveAnimList.Count - 1) finFlag = true;
                });
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finFlag);
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
                           anim.onComplete();
                           if (i >= MoveAnimList.Count - 1) {
                               finFlag = true;
                               anim.target.DOKill();
                           }
                       });
        }
        MoveAnimList.Clear();
        yield return new WaitUntil(() => finFlag);
    }
}

public struct TweenMoveAnim{
    public Transform target;
    public Vector3 purpose;
    public float duration;
    public Action onComplete;

    public TweenMoveAnim(Transform target, Vector3 purpose, float duration, Action onComplete) {
        this.target = target;
        this.purpose = purpose;
        this.duration = duration;
        this.onComplete = onComplete;
    }
}
