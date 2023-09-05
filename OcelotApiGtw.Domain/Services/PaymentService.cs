using Microsoft.IdentityModel.Tokens;
using OcelotApiGtw.Domain.Interfaces;
using OcelotApiGtw.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcelotApiGtw.Domain.Services
{
    public class PaymentService : IPaymentService
    {
        public AuthToken GenerateToken(AuthUser user)
        {
            var key = new SymmetricSecurityKey
             (Encoding.UTF8.GetBytes("payment_api_secret"));
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
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: credentials);

            var authToken = new AuthToken();
            authToken.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authToken.ExpirationDate = expirationDate;

            return authToken;
        }
    }
}
