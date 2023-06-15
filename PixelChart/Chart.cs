using PixelChart.Model;
using System.Diagnostics;

namespace PixelChart;

internal class Chart
{
    //size variables
    public int chartAreaHeight = 300;
    public int chartAreaWidth = 1600;

    //data variables
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

    //painting
    public void Render(int width, int height)
    {
        Stopwatch t = new Stopwatch();
        t.Start();


        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);
    }
}
