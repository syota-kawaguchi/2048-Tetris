using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language {
    JA,
    EN
}

public class Strings : MonoBehaviour
{
    public static string FLICK(Language ln) => ln == Language.JA ? "フリック" : "Flick";
    public static string SWIPE(Language ln) => ln == Language.JA ? "スワイプ" : "Swipe";
    public static string TAP(Language ln) => ln == Language.JA ? "タップ" : "Tap";
}
