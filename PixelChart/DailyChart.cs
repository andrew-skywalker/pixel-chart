using PixelChart.Model;
using System.Diagnostics;
using System.Drawing;

namespace PixelChart;

public class DailyChart
{
    public DailyChart()
    {
        ColorScheme.Init("light");
    }

    //size variables
    public int chartAreaHeight = 300;
    public int chartAreaWidth = 1600;
    public int LeftPadding { get; set; } = 1;
    int candleWidth = 3;
    int candleAreaWidth = 4;

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
    public void Render(int width, int height)
    {
        Stopwatch t = new Stopwatch();
        t.Start();

        using var bmp = new Bitmap(width, height);
        using var gfx = Graphics.FromImage(bmp);
        using var penGreen = new Pen(ColorScheme.colorGreenCandle);
        using var penRed = new Pen(ColorScheme.colorRedCandle);
        using var penGray = new Pen(ColorScheme.colorGrid);
        using var dotGrayDot = new Pen(ColorScheme.colorGrid);
        dotGrayDot.DashPattern = ColorScheme.dashPattern;

        gfx.Clear(ColorScheme.colorBackground);

        //draw vertical grid behind candles
        foreach ((int x, _) in XTicks)
        {
            gfx.DrawLine(dotGrayDot, CoordToPixelX(x), 0, CoordToPixelX(x), chartAreaHeight);
        }

        // candles
        foreach (var c in Candles)
        {
            if (c.Close > c.Open)
            {
                DrawCandle(gfx, c.X, c, penGreen);
            }
            else
            {
                DrawCandle(gfx, c.X, c, penRed);
            }
        }

        // horizontal axis
        gfx.DrawLine(penGray, 0, chartAreaHeight, chartAreaWidth, chartAreaHeight);

        //vertical axis
        gfx.DrawLine(penGray, chartAreaWidth, 0, chartAreaWidth, chartAreaHeight);

        bmp.Save("daily_chart.png", System.Drawing.Imaging.ImageFormat.Png);

        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);
    }

    void DrawCandle(Graphics gfx, int i, OhlcCandle candle, Pen pen)
    {
        int rectX = LeftPadding + candleAreaWidth * i;
        int rectY, rectHeight;

        //TODO: doji case is not drawn when close!=open, yet body height is too small

        //body
        if (candle.Close == candle.Open) //TODO: doji case might be detected by rectHeight, not OHLC
        {
            rectY = CoordToPixelY(candle.Close);
            rectHeight = 0;

            gfx.DrawLine(pen, rectX, rectY, rectX + candleWidth - 1, rectY);
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
                Rectangle rect = new(rectX, rectY, candleWidth, rectHeight);
                gfx.FillRectangle(pen.Brush, rect);
            }
            else
            {
                Rectangle rect = new(rectX, rectY, candleWidth - 1, rectHeight);
                gfx.DrawRectangle(pen, rect);
            }
        }

        //upper wick
        int wickHigh = CoordToPixelY(candle.High);
        gfx.DrawLine(pen, rectX + 1, wickHigh, rectX + 1, rectY);

        //lower wick
        int wickLow = CoordToPixelY(candle.Low);
        gfx.DrawLine(pen, rectX + 1, rectY + rectHeight, rectX + 1, wickLow);
    }
}
