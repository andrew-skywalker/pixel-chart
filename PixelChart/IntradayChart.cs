using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelChart;

public class IntradayChart : Chart
{
    public IntradayChart(string theme = "light") :
        base(theme)
    {
        
    }

    public override SKBitmap Render(int width, int height)
    {
        throw new NotImplementedException();
    }
}
