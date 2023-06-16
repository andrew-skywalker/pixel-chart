using PixelChart.Model;
using System.Diagnostics;

namespace PixelChart;

internal class DailyChart
{
    //size variables
    public int chartAreaHeight = 300;
    public int chartAreaWidth = 1600;
    public int LeftPadding { get; set; } = 1;
    int candleWidth = 3;
    int candleAreaWidth = 4;

    //data variables
    decimal y_min;
    decimal y_max;
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


        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);
    }
}
