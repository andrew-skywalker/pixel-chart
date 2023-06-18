using PixelChart.Interface;

namespace PixelChart.Model;

public class PriceLine : IPlottable
{
    public decimal Price { get; set; }
    public DateTime DtFrom { get; set; }
    public DateTime DtTo { get; set; }
}
