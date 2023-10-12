using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Seamstress.Application.Contracts;
using Seamstress.Application.Dtos;
using Seamstress.Domain.Identity;

namespace Seamstress.Application
{
  public class TokenService : ITokenService
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration,
                        UserManager<User> userManager,
                        IMapper mapper)
    {
      this._mapper = mapper;
      this._configuration = configuration;
      this._userManager = userManager;
      this._key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[Environment.GetEnvironmentVariable("TOKEN_KEY")]));
    }

    public async Task<string> CreateToken(UserUpdateDto userUpdateDto)
    {
      try
      {
        var user = _mapper.Map<User>(userUpdateDto);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
          new(ClaimTypes.NameIdentifier, user.Id.ToString()),
          new(ClaimTypes.Name, user.UserName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescription = new SecurityTokenDescriptor
        {
          Subject = new ClaimsIdentity(claims),
          Expires = DateTime.Now.AddDays(1),
          SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescription);

        return tokenHandler.WriteToken(token);
      }
      catch (Exception ex)
      {

        throw new Exception($"Erro ao gerar token. Erro: {ex.Message}");
      }
    }
  }
}