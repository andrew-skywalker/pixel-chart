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

    //data variables
    decimal y_min;
    decimal y_max;
    public List<(int x, string text)> XTicks = new();
    public Dictionary<DateTime, int> DateToCoordDict = new();

    List<OhlcCandle> _candles = new();
    public List<OhlcCandle> Candles
    {
        get => _candles;
        set
        {
            if (_candles != value)
            {
                DateToCoordDict.Clear();

                foreach (var candle in value)
                {
                    if (!DateToCoordDict.ContainsKey(candle.Dt))
                    {
                        DateToCoordDict.Add(candle.Dt, candle.X);
                    }
                }

                _candles = value;
            }

            AutoScaleY();
        }
    }

    //utility methods
    public void AutoScaleY()
    {
        y_max = Candles.Max(x => x.High);
        y_min = Candles.Min(x => x.Low);
    }

    int CoordToPixelY(decimal coord)
    {
        decimal range = y_max - y_min;

        decimal percentFromHigh = (y_max - coord) / range;

        int pixel = (int)(chartAreaHeight * percentFromHigh);

        return pixel;
    }

    public int CoordToPixelX(int x)
    {
        int pixel = LeftPadding + candleAreaWidth * x + 1;
        return pixel;
    }

    //painting
    public void Render(int width, int height, string filename)
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

        SKFileWStream fs = new(filename);
        bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
    }

    void DrawCandle(SKCanvas canvas, OhlcCandle candle, SKPaint paint)
    {
        int rectX = LeftPadding + candleAreaWidth * candle.X;
        int rectY, rectHeight;

        //TODO: doji case is not drawn when close!=open, yet body height is too small

        //body
        if (candle.Close == candle.Open) //TODO: doji case might be detected by rectHeight, not OHLC
        {
            rectY = CoordToPixelY(candle.Close);
            rectHeight = 0;

            canvas.DrawLine(rectX, rectY, rectX + candleWidth - 1, rectY, paint);
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
}
