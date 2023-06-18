using PixelChart.Interface;
using SkiaSharp;

namespace PixelChart.Plottables;

public class YAxis : IPlottable
{
    public bool IsVisible { get; set; } = true;
    public bool IsGridVisible { get; set; } = true;

    public List<decimal> YTicks = new();

    public void Plot(SKCanvas canvas, Chart p)
    {
        //vertical axis
        if (IsVisible)
        {
            canvas.DrawLine(p.chartAreaWidth, p.CandleAreaTopPixel, p.chartAreaWidth, p.CandleAreaBottomPixel, p.paintAxes);
        }

        //horizontal grid
        if (IsGridVisible)
        {
            foreach (decimal yTick in YTicks)
            {
                canvas.DrawLine(0, p.CoordToPixelY(yTick), p.chartAreaWidth - 1, p.CoordToPixelY(yTick), p.paintHorizontalGrid);
            }
        }

        //ticks
        foreach (decimal yTick in YTicks)
        {
            int x = p.chartAreaWidth + 1;
            int y = p.CoordToPixelY(yTick) + 6;
            canvas.DrawText(yTick.ToString("0.00"), new SKPoint(x, y), p.paintLabels);
        }
    }
}
