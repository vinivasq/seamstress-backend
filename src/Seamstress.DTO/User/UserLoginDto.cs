namespace Seamstress.DTO
{
  public class UserLoginDto
  {
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
  }
}