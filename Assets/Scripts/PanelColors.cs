using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

public class PanelColors : MonoBehaviour {
    static readonly Color32 color1 = new Color32(60, 58, 50, 255);
    static readonly Color32 color2 = new Color32(236, 226, 216, 255);
    static readonly Color32 color4 = new Color32(236, 224, 198, 255);
    static readonly Color32 color8 = new Color32(237, 177, 127, 255);
    static readonly Color32 color16 = new Color32(235, 138, 83, 255);
    static readonly Color32 color32 = new Color32(240, 128, 104, 255);
    static readonly Color32 color64 = new Color32(229, 91, 52, 255);
    static readonly Color32 color128 = new Color32(244, 218, 108, 255);
    static readonly Color32 color256 = new Color32(241, 206, 76, 255);
    static readonly Color32 color512 = new Color32(221, 194, 41, 255);
    static readonly Color32 color1024 = new Color32(227, 187, 12, 255);
    static readonly Color32 color2048 = new Color32(235, 195, 2, 255);
    static readonly Color32 color4096 = new Color32(105, 215, 142, 255);
    static readonly Color32 color8192 = new Color32(62, 171, 170, 255);
    static readonly Color32 color16384 = new Color32(27, 139, 176, 255);
    static readonly Color32 color32768 = new Color32(0, 100, 195, 255);
    static readonly Color32 color65536 = new Color32(32, 0, 195, 255);
    static readonly Color32 color131072 = new Color32(154, 0, 195, 255);
    static readonly Color32 darkBrown = new Color32(108, 98, 91, 255);
    static readonly Color32 white = new Color32(255, 255, 255, 255);

    public static Color GetColor(int i) {
        switch (i % 18) {
            case 1: return color2;
            case 2: return color4;
            case 3: return color8;
            case 4: return color16;
            case 5: return color32;
            case 6: return color64;
            case 7: return color128;
            case 8: return color256;
            case 9: return color512;
            case 10: return color1024;
            case 11: return color2048;
            case 12: return color4096;
            case 13: return color8192;
            case 14: return color16384;
            case 15: return color32768;
            case 16: return color65536;
            case 17: return color131072;
            case 0: return color1;
            default: return color1;
        }
    }

    public static Color GetTextColor(int i) {
        if (i <= 2) return darkBrown;
        else return Color.white;
    }
}
