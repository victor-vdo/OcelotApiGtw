using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OcelotApiGtw.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        public AuthController(IOrderService orderService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Order([FromBody] AuthUser authUser)
        {
            var order = _orderService.GenerateToken(authUser);
            return Ok(order);
        }

    }

    public interface IPaymentService
    {
        AuthToken GenerateToken(AuthUser user);
    }

    public interface IOrderService
    {
        AuthToken GenerateToken(AuthUser user);
    }

    public class PaymentService : IPaymentService
    {
        public AuthToken GenerateToken(AuthUser user)
        {
            var key = new SymmetricSecurityKey
 (Encoding.UTF8.GetBytes("H98HF9QD8HF928H9F8H293H89qh9hf9ahf98hH89="));
            var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username.
                                                    ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.
                                            NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                    audience: "paymentAudience",
                    issuer: "paymentIssuer",
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

    public class OrderService : IOrderService
    {
        public AuthToken GenerateToken(AuthUser user)
        {
            var key = new SymmetricSecurityKey
 (Encoding.UTF8.GetBytes("H98HF9QD8HF928H9F8H293H89qh9hf9ahf98hH89="));
            var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username.
                                                    ToString()),
                new Claim("scheme","order_auth_scheme"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.
                                            NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                    audience: "orderAudience",
                    issuer: "orderIssuer",
                    claims: claims,
                    expires: expirationDate,
                    signingCredentials: credentials);
            var authToken = new AuthToken();
            authToken.Token = new JwtSecurityTokenHandler().
                                            WriteToken(token);
            authToken.ExpirationDate = expirationDate;

            return authToken;
        }

        private string GerarTokenJWT(string key)
        {
            var issuer = "Jwt:Issuer";
            var audience = "Jwt:Audience";
            var expiry = DateTime.Now.AddMinutes(120);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: issuer, audience: audience,
                    expires: DateTime.Now.AddMinutes(120), signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
    }

    public class AuthToken
    {
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }

    public class AuthUser
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

}
