using System.Diagnostics;
using SkiaSharp;

namespace PixelChart;

public class DailyChart : Chart
{
    public DailyChart(string theme = "light") :
        base(theme)
    {
        
    }

    //drawing
    override public SKBitmap Render(int width, int height)
    {
        Stopwatch t = new();
        t.Start();

        SKBitmap bmp = new(width, height);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(skBackgroud);

        DrawAxesAndGrids(canvas);
        
        DrawCandles(canvas);

        DrawTitle(canvas);

        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);

        return bmp;
    }
}
