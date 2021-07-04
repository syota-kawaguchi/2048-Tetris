using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

public class PanelColors : MonoBehaviour {
    static readonly Color color1 = new Color(60, 58, 50);
    static readonly Color color2 = new Color(236, 226, 216);
    static readonly Color color4 = new Color(236, 224, 198);
    static readonly Color color8 = new Color(237, 177, 127);
    static readonly Color color16 = new Color(235, 138, 83);
    static readonly Color color32 = new Color(240, 128, 104);
    static readonly Color color64 = new Color(229, 91, 52);
    static readonly Color color128 = new Color(244, 218, 108);
    static readonly Color color256 = new Color(241, 206, 76);
    static readonly Color color512 = new Color(221, 194, 41);
    static readonly Color color1024 = new Color(227, 187, 12);
    static readonly Color color2048 = new Color(235, 195, 2);
    static readonly Color color4096 = new Color(105, 215, 142);

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
