using PixelChart.Interface;
using PixelChart.Plottables;
using SkiaSharp;

namespace PixelChart;

public abstract class Chart
{
    public Chart(string theme)
    {
        ColorScheme.Init(theme);

        skBackgroud = SKColorFromSystemDrawing(ColorScheme.colorBackground);
        paintBackgroudNonMh = SKPaintFromSystemDrawing(ColorScheme.colorBackgroundNonMh);
        paintGreen = SKPaintFromSystemDrawing(ColorScheme.colorGreenCandle);
        paintRed = SKPaintFromSystemDrawing(ColorScheme.colorRedCandle);
        paintYellow = SKPaintFromSystemDrawing(ColorScheme.colorVerticalLine);

        paintAxes = SKPaintFromSystemDrawing(ColorScheme.colorAxes);
        paintVerticalGrid = SKPaintFromSystemDrawing(ColorScheme.colorVerticalGrid);
        paintVerticalGrid.PathEffect = SKPathEffect.CreateDash(ColorScheme.dashPattern, 0);
        paintHorizontalGrid = SKPaintFromSystemDrawing(ColorScheme.colorHorizontalGrid);
        paintHorizontalGrid.PathEffect = SKPathEffect.CreateDash(ColorScheme.dashPattern, 0);

        paintLabels = SKPaintFromSystemDrawing(ColorScheme.colorLables);
        paintLabels.Typeface = SKTypeface.FromFamilyName("Segoe UI");
        paintLabels.TextSize = labelsFontSize;
        paintLabels.IsAntialias = true;

        paintTitle = SKPaintFromSystemDrawing(ColorScheme.colorTitle);
        paintTitle.Typeface = SKTypeface.FromFamilyName("Segoe UI",
            SKFontStyleWeight.Bold,
            SKFontStyleWidth.Normal,
            SKFontStyleSlant.Upright);
        paintTitle.TextSize = titleFontSize;
        paintTitle.IsAntialias = true;
    }

    //SKPaint
    internal readonly SKColor skBackgroud;
    internal readonly SKPaint paintBackgroudNonMh;
    internal readonly SKPaint paintGreen;
    internal readonly SKPaint paintRed;
    internal readonly SKPaint paintYellow;

    internal readonly SKPaint paintAxes;
    internal readonly SKPaint paintVerticalGrid;
    internal readonly SKPaint paintHorizontalGrid;

    internal readonly SKPaint paintLabels;
    internal readonly SKPaint paintTitle;

    //size variables
    public int TitleHeight = 0;
    public string Title = "";
    public int chartAreaHeight = 300;
    public int chartAreaWidth = 1600;
    public int LeftPadding { get; set; } = 1;
    internal int candleWidth = 3;
    internal int candleAreaWidth = 4;
    internal int labelsFontSize = 16;
    internal int titleFontSize = 30;

    static SKColor SKColorFromSystemDrawing(System.Drawing.Color source)
    {
        return new SKColor(red: source.R, green: source.G, blue: source.B);
    }

    static SKPaint SKPaintFromSystemDrawing(System.Drawing.Color source)
    {
        return new SKPaint
        {
            Color = SKColorFromSystemDrawing(source)
        };
    }

    //data variables
    decimal y_min;
    decimal y_max;
    public List<IPlottable> Plottables { get; set; } = new();
    List<OhlcCandle> _candles = new();
    public List<OhlcCandle> Candles
    {
        get => _candles;
        set
        {
            if (_candles != value)
            {
                FillDateToCoordDict(value);
                _candles = value;
            }

            AutoScaleY();
        }
    }

    public XAxis XAxis { get; set; } = new();
    public YAxis YAxis { get; set; } = new();

    public Dictionary<DateTime, int> DateToCoordDict = new();
    
    //utility methods
    public void AutoScaleY()
    {
        if (Candles.Count == 0)
        {
            return;
        }

        y_max = Candles.Max(x => x.High);
        y_min = Candles.Min(x => x.Low);

        DefineYTicks();
    }

    public void DefineYTicks()
    {
        decimal tickStep = DefineTickStep();

        //add ticks
        decimal currTick = (int)Math.Ceiling(y_min / tickStep) * tickStep;
        while (currTick < y_max)
        {
            YAxis.YTicks.Add(currTick);
            currTick += tickStep;
        }
    }

    const int desiredLabelCount = 4;
    decimal DefineTickStep()
    {
        List<decimal> steps = new() 
        {
                 0.02m,       0.05m, 
            0.1m, 0.2m, 0.25m, 0.5m,
            1,      2,           5,
            10,     20,   25,    50,
            100,    200,  250,   500, 1000, 10000
        };

        decimal range = y_max - y_min;
        decimal stepEstimate = (range / desiredLabelCount);
        decimal closest = steps.OrderBy(x => Math.Abs(stepEstimate - x)).First();

        return closest;
    }

    internal int CoordToPixelY(decimal coord)
    {
        decimal range = y_max - y_min;

        decimal percentFromHigh = (y_max - coord) / range;

        int pixel = TitleHeight + (int)(chartAreaHeight * percentFromHigh);

        return pixel;
    }

    internal int CoordToPixelX(int x)
    {
        int pixel = LeftPadding + candleAreaWidth * x + 1;
        return pixel;
    }

    internal int CandleAreaTopPixel { get => TitleHeight; }
    internal int CandleAreaBottomPixel { get => TitleHeight + chartAreaHeight; }

    private void FillDateToCoordDict(List<OhlcCandle> candles)
    {
        DateToCoordDict.Clear();

        foreach (var candle in candles)
        {
            if (!DateToCoordDict.ContainsKey(candle.Dt))
            {
                DateToCoordDict.Add(candle.Dt, candle.X);
            }
        }
    }

    //drawing
    internal void DrawAxesAndGrids(SKCanvas canvas)
    {
        YAxis.Plot(canvas, this);
        XAxis.Plot(canvas, this);
    }

    internal void DrawCandles(SKCanvas canvas)
    {
        foreach (OhlcCandle c in Candles)
        {
            c.Plot(canvas, this);
        }
    }

    public void DrawPlottables(SKCanvas canvas)
    {
        foreach (var item in Plottables)
        {
            item.Plot(canvas, this);
        }
    }

    public void DrawTitle(SKCanvas canvas)
    {
        if (TitleHeight == 0 || Title == "")
        {
            return;
        }

        canvas.DrawText(Title, new SKPoint(20, titleFontSize - 5), paintTitle);
    }

    public abstract SKBitmap Render(int width, int height); //contains Daily/Intraday logic

    public void RenderToFile(int width, int height, string filename)
    {
        SKBitmap bmp = Render(width, height);

        using SKFileWStream fs = new(filename);
        bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
    }
}
