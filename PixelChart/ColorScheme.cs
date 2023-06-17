using System.Drawing;

namespace PixelChart;

internal class ColorScheme
{
    //current values
    public static Color colorBackground;
    public static Color colorBackgroundNonMh;
    public static Color colorGreenCandle;
    public static Color colorRedCandle;
    public static Color colorGrid;
    public static List<Color> labelColors = new() { };
    public static Color colorLables;
    public static float[] dashPattern = new float[] {};
    public static bool isCandlesFilled;

    public static void Init(string theme = "light")
    {
        SetColorScheme(theme);
    }

    //saved colors
    public static void SetColorScheme(string scheme)
    {
        switch (scheme)
        {
            case "light":
                colorBackground = Color.FromArgb(250, 250, 250);
                colorBackgroundNonMh = Color.FromArgb(235, 235, 235);
                colorGreenCandle = Color.FromArgb(0, 110, 0);
                colorRedCandle = Color.FromArgb(110, 0, 0);
                colorGrid = Color.Black;
                labelColors = new() { Color.FromArgb(46, 124, 237), Color.Red, Color.Green, Color.DarkGoldenrod, Color.Black };
                colorLables = Color.Black;
                dashPattern = new float[] { 2, 23 };
                isCandlesFilled = true;
                break;

            case "navy":
                colorBackground = Color.Navy;
                colorBackgroundNonMh = Color.DarkBlue;
                colorGreenCandle = Color.FromArgb(0, 235, 0);
                colorRedCandle = Color.FromArgb(235, 0, 0);
                colorGrid = Color.DarkGray;
                colorLables = Color.DarkGray;
                dashPattern = new float[] { 1, 25 };
                isCandlesFilled = false;
                break;

            default:
                throw new Exception("Color Theme is unknown");
        }
    }
}
