using PixelChart.Interface;
using SkiaSharp;

namespace PixelChart.Plottables;

public class OhlcCandle : IPlottable
{
    public int X { get; set; }
    public DateTime Dt { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }

    public void Plot(SKCanvas canvas, Chart p)
    {
        SKPaint paint = Close >= Open ? p.paintGreen : p.paintRed;

        int rectX = p.LeftPadding + p.candleAreaWidth * X;
        int rectY, rectHeight;

        //body
        if (Close > Open)
        {
            rectY = p.CoordToPixelY(Close);
            rectHeight = p.CoordToPixelY(Open) - rectY;
        }
        else
        {
            rectY = p.CoordToPixelY(Open);
            rectHeight = p.CoordToPixelY(Close) - rectY;
        }

        if (rectHeight == 0) //doji case
        {
            canvas.DrawLine(rectX, rectY, rectX + p.candleWidth, rectY, paint);
        }
        else
        {
            if (ColorScheme.isCandlesFilled)
            {
                SKRect rect = new(rectX, rectY, rectX + p.candleWidth, rectY + rectHeight);
                canvas.DrawRect(rect, paint);
            }
            else
            {
                SKRect rect = new(rectX, rectY, rectX + p.candleWidth - 1, rectY + rectHeight);
                canvas.DrawRect(rect, paint);
            }
        }

        //upper wick
        int wickHigh = p.CoordToPixelY(High);
        canvas.DrawLine(rectX + 1, wickHigh, rectX + 1, rectY, paint);

        //lower wick
        int wickLow = p.CoordToPixelY(Low);
        canvas.DrawLine(rectX + 1, rectY + rectHeight, rectX + 1, wickLow, paint);
    }
}
