using System.ComponentModel.DataAnnotations;
using Seamstress.Domain.Enum;

namespace Seamstress.Application.Dtos
{
  public class OrderInputDto
  {
    public int Id { get; set; }
    public string? Description { get; set; }


    [Display(Name = "Data de Criação")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    [DataType(DataType.DateTime, ErrorMessage = "{0} inválida")]
    public DateTime CreatedAt { get; set; }


    [Display(Name = "Prazo Final")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    [DataType(DataType.DateTime, ErrorMessage = "{0} inválido")]
    public DateTime Deadline { get; set; }

    [Display(Name = "Data do pedido")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    [DataType(DataType.DateTime, ErrorMessage = "{0} inválido")]
    public DateTime OrderedAt { get; set; }

    [Display(Name = "Valor Total")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    public decimal Total { get; set; }


    public Step Step { get; set; } = Step.Aguardando;


    [Display(Name = "Cliente")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    public int CustomerId { get; set; }

    [Display(Name = "Executor")]
    [Required(ErrorMessage = "O Campo {0} não pode ficar vazio.")]
    public int ExecutorId { get; set; }

    public IEnumerable<ItemOrderInputDto> ItemOrders { get; set; } = null!;
  }
}