using PixelChart.Model;
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

    //data variables
    decimal y_min;
    decimal y_max;
    List<OhlcCandle> _candles = new();
    public List<OhlcCandle> Candles
    {
        get => _candles;
        set
        {
            if (_candles != value)
            {
                _candles = value;
            }

            AutoScaleY();
        }
    }

    public List<(int x, string text)> XTicks = new();

    //utility methods
    public void AutoScaleY()
    {
        y_max = Candles.Max(x => x.High);
        y_min = Candles.Min(x => x.Low);
    }

    internal int CoordToPixelY(decimal coord)
    {
        decimal range = y_max - y_min;

        decimal percentFromHigh = (y_max - coord) / range;

        int pixel = (int)(chartAreaHeight * percentFromHigh);

        return pixel;
    }

    internal int CoordToPixelX(int x)
    {
        int pixel = LeftPadding + candleAreaWidth * x + 1;
        return pixel;
    }
}
