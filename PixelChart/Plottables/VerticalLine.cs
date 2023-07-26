using PixelChart.Interface;
using SkiaSharp;

namespace PixelChart.Plottables;

public class VerticalLine : IPlottable
{
    public DateTime Dt { get; set; }

    public void Plot(SKCanvas canvas, Chart p)
    {
        int x = p.CoordToPixelX(p.DateToCoordDict[Dt]);

        int y1 = p.CandleAreaTopPixel;
        int y2 = p.CandleAreaBottomPixel;

        canvas.DrawLine(x, y1, x, y2, p.paintYellow);
    }
}