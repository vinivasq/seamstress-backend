namespace Seamstress.Application.Dtos
{
  public class UserDto
  {
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
  }
}