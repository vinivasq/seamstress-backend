namespace Seamstress.Domain
{
  public class Customer
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Neighborhood { get; set; } = null!;
    public int Number { get; set; }
    public string? Complement { get; set; }
    public string Cep { get; set; } = null!;
    public string CPF_CNPJ { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Sizings? Sizings { get; set; }
  }
}