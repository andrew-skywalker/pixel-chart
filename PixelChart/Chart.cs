using PixelChart.Model;

namespace PixelChart;

internal class Chart
{
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

    public void Render(int width, int height)
    {

    }
}
