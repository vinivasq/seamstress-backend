namespace Seamstress.Domain
{
    public class ImportMapping
    {
        public int Id { get; set; }
        public int SalePlatformId { get; set; }
        public SalePlatform SalePlatform { get; set; } = null!;
        public string MappingsJson { get; set; } = null!;
    }
}
