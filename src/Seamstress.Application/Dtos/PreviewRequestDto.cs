using Seamstress.Application;

namespace Seamstress.Application.Dtos
{
    public class PreviewRequestDto
    {
        public List<NormalizedProduct> Products { get; set; } = new();
        public int SalePlatformId { get; set; }
    }
}
