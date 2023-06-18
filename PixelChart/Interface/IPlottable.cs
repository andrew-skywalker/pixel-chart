using SkiaSharp;

namespace PixelChart.Interface;

public interface IPlottable
{
    void Plot(SKCanvas canvas, Chart p);
}
