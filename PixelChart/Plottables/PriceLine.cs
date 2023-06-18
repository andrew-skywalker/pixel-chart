using PixelChart.Interface;
using SkiaSharp;

namespace PixelChart.Plottables;

public class PriceLine : IPlottable
{
    public decimal Price { get; set; }
    public DateTime DtFrom { get; set; }
    public DateTime DtTo { get; set; }

    public void Plot(SKCanvas canvas, Chart p)
    {
        if (p.DateToCoordDict.ContainsKey(DtFrom) && p.DateToCoordDict.ContainsKey(DtTo))
        {
            int x1 = p.CoordToPixelX(p.DateToCoordDict[DtFrom]);
            int x2 = p.CoordToPixelX(p.DateToCoordDict[DtTo]);
            int y = p.CoordToPixelY(Price);

            canvas.DrawLine(x1, y, x2, y, p.paintRed);
        }
    }
}
