using PixelChart.Model;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelChart;

public abstract class Chart
{
    public Chart(string theme)
    {
        ColorScheme.Init(theme);

        skBackgroud = SKColorFromSystemDrawing(ColorScheme.colorBackground);
        paintBackgroudNonMh = SKPaintFromSystemDrawing(ColorScheme.colorBackgroundNonMh);
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
    internal readonly SKPaint paintBackgroudNonMh;
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

    //drawing
    internal void DrawVerticalGrid(SKCanvas canvas)
    {
        //draw vertical grid behind candles
        foreach ((int x, _) in XTicks)
        {
            canvas.DrawLine(CoordToPixelX(x), 0, CoordToPixelX(x), chartAreaHeight, paintGrayDot);
        }
    }

    internal void DrawAxes(SKCanvas canvas)
    {
        //horizontal axis
        canvas.DrawLine(0, chartAreaHeight, chartAreaWidth, chartAreaHeight, paintGray);

        //vertical axis
        canvas.DrawLine(chartAreaWidth, 0, chartAreaWidth, chartAreaHeight, paintGray);
    }

    internal void DrawCandle(SKCanvas canvas, OhlcCandle candle, SKPaint paint)
    {
        int rectX = LeftPadding + candleAreaWidth * candle.X;
        int rectY, rectHeight;

        //body
        if (candle.Close > candle.Open)
        {
            rectY = CoordToPixelY(candle.Close);
            rectHeight = CoordToPixelY(candle.Open) - rectY;
        }
        else
        {
            rectY = CoordToPixelY(candle.Open);
            rectHeight = CoordToPixelY(candle.Close) - rectY;
        }

        if (rectHeight == 0) //doji case
        {
            canvas.DrawLine(rectX, rectY, rectX + candleWidth, rectY, paint);
        }
        else
        {
            if (ColorScheme.isCandlesFilled)
            {
                SKRect rect = new(rectX, rectY, rectX + candleWidth, rectY + rectHeight);
                canvas.DrawRect(rect, paint);
            }
            else
            {
                SKRect rect = new(rectX, rectY, rectX + candleWidth - 1, rectY + rectHeight);
                canvas.DrawRect(rect, paint);
            }
        }

        //upper wick
        int wickHigh = CoordToPixelY(candle.High);
        canvas.DrawLine(rectX + 1, wickHigh, rectX + 1, rectY, paint);

        //lower wick
        int wickLow = CoordToPixelY(candle.Low);
        canvas.DrawLine(rectX + 1, rectY + rectHeight, rectX + 1, wickLow, paint);
    }

    internal void DrawCandles(SKCanvas canvas)
    {
        foreach (OhlcCandle c in Candles)
        {
            if (c.Close >= c.Open)
            {
                DrawCandle(canvas, c, paintGreen);
            }
            else
            {
                DrawCandle(canvas, c, paintRed);
            }
        }
    }

    internal void DrawXAxisTicks(SKCanvas canvas)
    {
        //draw labels
        foreach ((int x, string text) in XTicks)
        {
            canvas.DrawText(text, new SKPoint(x * candleAreaWidth, chartAreaHeight + fontSize), paintLabels);
        }
    }

    public abstract SKBitmap Render(int width, int height); //contains Daily/Intraday logic

    public void RenderToFile(int width, int height, string filename)
    {
        SKBitmap bmp = Render(width, height);

        using SKFileWStream fs = new(filename);
        bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
    }
}
