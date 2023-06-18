using PixelChart.Model;
using SkiaSharp;
using System.Diagnostics;

namespace PixelChart;

public class IntradayChart : Chart
{
    public IntradayChart(string theme = "light") :
        base(theme)
    {
        
    }

    readonly TimeSpan mhStartTime = new(9, 30, 0);
    readonly TimeSpan mhEndTime = new(16, 0, 0);

    public override SKBitmap Render(int width, int height)
    {
        Stopwatch t = new();
        t.Start();

        SKBitmap bmp = new(width, height);
        using SKCanvas canvas = new(bmp);
        canvas.Clear(skBackgroud);

        //draw background for each candle
        foreach (OhlcCandle c in Candles)
        {
            int rectX = LeftPadding + candleAreaWidth * c.X - 1;

            SKRect rect = new(rectX, 0, rectX + candleAreaWidth, chartAreaHeight);
            
            if (c.Dt.TimeOfDay < mhStartTime || c.Dt.TimeOfDay >= mhEndTime) //non-market hours
            {
                canvas.DrawRect(rect, paintBackgroudNonMh);
            }
        }

        DrawPlottables(canvas);

        DrawHorizontalGrid(canvas);
        DrawVerticalGrid(canvas);

        DrawCandles(canvas);

        DrawAxes(canvas);

        DrawXAxisTicks(canvas);
        DrawYAxisTicks(canvas);

        t.Stop();
        Debug.WriteLine(t.ElapsedMilliseconds);

        return bmp;
    }
}
