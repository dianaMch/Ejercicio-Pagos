using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace PAGO.WEBSERVICE.Data
{
    public class JwtValidator
    {
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Jwt__Secret"]);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = ConfigurationManager.AppSettings["Jwt__TokenValidationParameters"],
                ValidAudience = ConfigurationManager.AppSettings["Jwt__TokenValidationParameters"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            return tokenHandler.ValidateToken(token, parameters, out validatedToken);
        }
    }
}