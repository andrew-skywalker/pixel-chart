using PixelChart.Model;
using System.Diagnostics;
using SkiaSharp;

namespace PixelChart;

public class DailyChart : Chart
{
    public DailyChart(string theme = "light") :
        base(theme)
    {
        
    }

    //painting
    public SKBitmap Render(int width, int height)
    {
        Stopwatch t = new();
        t.Start();

        SKBitmap bmp = new(width, height);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(skBackgroud);

        //draw vertical grid behind candles
        foreach ((int x, _) in XTicks)
        {
            canvas.DrawLine(CoordToPixelX(x), 0, CoordToPixelX(x), chartAreaHeight, paintGrayDot);
        }

        // candles
        foreach (var c in Candles)
        {
            if (c.Close > c.Open)
            {
                DrawCandle(canvas, c, paintGreen);
            }
            else
            {
                DrawCandle(canvas, c, paintRed);
            }
        }

        // horizontal axis
        canvas.DrawLine(0, chartAreaHeight, chartAreaWidth, chartAreaHeight, paintGray);

        //vertical axis
        canvas.DrawLine(chartAreaWidth, 0, chartAreaWidth, chartAreaHeight, paintGray);

        //draw labels
        foreach ((int x, string text) in XTicks)
        {
            canvas.DrawText(text, new SKPoint(x * candleAreaWidth, chartAreaHeight + fontSize), paintLabels);
        }

        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);

        return bmp;
    }

    public void RenderToFile(int width, int height, string filename)
    {
        SKBitmap bmp = Render(width, height);

        using SKFileWStream fs = new(filename);
        bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
    }

    void DrawCandle(SKCanvas canvas, OhlcCandle candle, SKPaint paint)
    {
        int rectX = LeftPadding + candleAreaWidth * candle.X;
        int rectY, rectHeight;

        //body
        if (candle.Close == candle.Open)
        {
            rectY = CoordToPixelY(candle.Close);
            rectHeight = 0;

            canvas.DrawLine(rectX, rectY, rectX + candleWidth - 1, rectY, paint); //not actually hit
        }
        else
        {
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
                canvas.DrawLine(rectX, rectY, rectX + candleWidth, rectY, paint); //working
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
        }

        //upper wick
        int wickHigh = CoordToPixelY(candle.High);
        canvas.DrawLine(rectX + 1, wickHigh, rectX + 1, rectY, paint);

        //lower wick
        int wickLow = CoordToPixelY(candle.Low);
        canvas.DrawLine(rectX + 1, rectY + rectHeight, rectX + 1, wickLow, paint);
    }
}
