using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PAGO.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PAGO.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        public IActionResult Logeo([FromBody] LoginDataInput loginDataInput)
        {
            //Simulacion de validacion de usuario
            if (loginDataInput.Usuario != _config["Auth:User"] || loginDataInput.Contraseña != _config["Auth:Pass"])
                 return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, loginDataInput.Usuario)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:ValidationParameters"],
                Audience = _config["Jwt:ValidationParameters"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new { token = tokenString });
        }
    }
}
