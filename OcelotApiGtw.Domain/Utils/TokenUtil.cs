using Microsoft.IdentityModel.Tokens;
using OcelotApiGtw.Domain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcelotApiGtw.Domain.Utils
{
    public static class TokenUtil
    {
        public static AuthToken GenerateToken(AuthUser user, string authenticationProviderKey)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationProviderKey));
            var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256Signature);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var claims = new[]
            {
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

        public static bool IsValid(string auth)
        {
            var user = new AuthUser()
            {
                Username = auth.Split("|")[0],
                Password = auth.Split("|")[1]
            };
            return true;
        }
    }
}
