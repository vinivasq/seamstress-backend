namespace Seamstress.Application.Dtos
{
  public class CustomerExportDto
  {
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Cep { get; set; } = null!;
    public string City { get; set; } = null!;
    public string UF { get; set; } = null!;
    public decimal OrdersTotal { get; set; }
  }
}