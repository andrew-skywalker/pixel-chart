using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelChart;

public class Chart
{
    public Chart(string theme)
    {
        ColorScheme.Init(theme);

        skBackgroud = SKColorFromSystemDrawing(ColorScheme.colorBackground);
        paintGreen = SKPaintFromSystemDrawing(ColorScheme.colorGreenCandle);
        paintRed = SKPaintFromSystemDrawing(ColorScheme.colorRedCandle);
        paintGray = SKPaintFromSystemDrawing(ColorScheme.colorGrid);

        paintGrayDot = SKPaintFromSystemDrawing(ColorScheme.colorGrid);
        paintGrayDot.PathEffect = SKPathEffect.CreateDash(ColorScheme.dashPattern, 0);

        paintLabels = SKPaintFromSystemDrawing(ColorScheme.colorLables);
        paintLabels.Typeface = SKTypeface.FromFamilyName("Segoe UI");
        paintLabels.TextSize = fontSize;
        paintLabels.IsAntialias = true;
    }

    //SKPaint
    internal readonly SKColor skBackgroud;
    internal readonly SKPaint paintGreen;
    internal readonly SKPaint paintRed;
    internal readonly SKPaint paintGray;
    internal readonly SKPaint paintGrayDot;
    internal readonly SKPaint paintLabels;

    //size variables
    public int chartAreaHeight = 300;
    public int chartAreaWidth = 1600;
    public int LeftPadding { get; set; } = 1;
    internal int candleWidth = 3;
    internal int candleAreaWidth = 4;
    internal int fontSize = 16;

    static SKColor SKColorFromSystemDrawing(System.Drawing.Color source)
    {
        return new SKColor(red: source.R, green: source.G, blue: source.B);
    }

    static SKPaint SKPaintFromSystemDrawing(System.Drawing.Color source)
    {
        return new SKPaint
        {
            Color = SKColorFromSystemDrawing(source)
        };
    }
}
