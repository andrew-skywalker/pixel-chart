using PixelChart.Interface;
using SkiaSharp;

namespace PixelChart.Plottables;

public class XAxis : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public bool IsGridVisible { get; set; } = true;

    public List<(int x, string text)> XTicks = new();

    public void Plot(SKCanvas canvas, Chart p)
    {
        //horizontal axis
        if (IsVisible)
        {
            canvas.DrawLine(0, p.CandleAreaBottomPixel, p.chartAreaWidth, p.CandleAreaBottomPixel, p.paintAxes);
        }

        //vertical grid
        if (IsGridVisible)
        {
            foreach ((int x, _) in XTicks)
            {
                canvas.DrawLine(p.CoordToPixelX(x), p.CandleAreaTopPixel, p.CoordToPixelX(x), 
                    p.CandleAreaBottomPixel - 1, p.paintVerticalGrid);
            }
        }

        //ticks
        foreach ((int x, string text) in XTicks)
        {
            canvas.DrawText(text, new SKPoint(x * p.candleAreaWidth, p.CandleAreaBottomPixel + p.labelsFontSize), 
                p.paintLabels);
        }
    }
}
