using Microsoft.IdentityModel.Tokens;
using OcelotApiGtw.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OcelotApiGtw.Domain.Services
{
    public class OrderService
    {
        public AuthToken GenerateToken(AuthUser user)
        {
            var key = new SymmetricSecurityKey
             (Encoding.UTF8.GetBytes("order_api_secret"));
            var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256Signature);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username.
                                                    ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.
                                            NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                    audience: "categoriesAudience",
                    issuer: "categoriesIssuer",
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: credentials);
            var authToken = new AuthToken();
            authToken.Token = new JwtSecurityTokenHandler().
                                            WriteToken(token);
            authToken.ExpirationDate = expirationDate;

            return authToken;
        }
    }
}
