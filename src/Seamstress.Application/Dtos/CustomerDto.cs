using System.ComponentModel.DataAnnotations;
using Seamstress.Domain;

namespace Seamstress.Application.Dtos
{
  public class CustomerDto
  {
    public int Id { get; set; }

    [Display(Name = "Nome")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Endereço")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    public string Address { get; set; } = null!;

    [Display(Name = "Cidade")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    public string City { get; set; } = null!;

    [Display(Name = "Bairro")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    public string Neighborhood { get; set; } = null!;

    [Display(Name = "Número")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    [Range(1, 99999, ErrorMessage = "O campo {0} deve estar entre 1 e 99999")]
    public int Number { get; set; }
    public string? Complement { get; set; }

    [Display(Name = "CEP")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "O campo {0} deve possuir 8 digitos")]
    public string Cep { get; set; } = null!;

    [Display(Name = "CPF/CNPJ")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    [RegularExpression(@"^(?:\d{11}|\d{14})$", ErrorMessage = "O campo {0} deve possuir 11 ou 14 digitos")]
    public string CPF_CNPJ { get; set; } = null!;

    [Display(Name = "Telefone")]
    [Required(ErrorMessage = "O campo {0} não pode ficar vazio.")]
    [Phone(ErrorMessage = "O campo {0} não é um número válido")]
    public string PhoneNumber { get; set; } = null!;

    [Display(Name = "E-mail")]
    [Required(ErrorMessage = "O Campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "Deve ser um {0} válido")]
    public string Email { get; set; } = null!;
    public Sizings? Sizings { get; set; }
  }
}