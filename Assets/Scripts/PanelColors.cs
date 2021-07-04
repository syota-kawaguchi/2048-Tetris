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

    public static Color GetColor(int i) {
        switch (i) {
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
            default: return color1;
        }
    }
}
