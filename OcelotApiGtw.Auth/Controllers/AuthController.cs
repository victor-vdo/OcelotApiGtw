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

        [HttpPost("payments")]
        public async Task<IActionResult> Payments([FromBody] AuthUser authUser)
        {
            var payment = _paymentService.GenerateToken(authUser);
            return Ok(payment);
        }

        [HttpPost("order")]
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
            var auth = "H98HF9QD8HF928H9F8H293H89qh9hf9ahf98hH89=";
            var enc = Encoding.ASCII.GetBytes(auth);
            var key = new SymmetricSecurityKey(enc);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expirationDate = DateTime.UtcNow.AddHours(2);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expirationDate,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var authToken = new AuthToken();
            authToken.Token = new JwtSecurityTokenHandler().WriteToken(token);
            authToken.ExpirationDate = expirationDate;

            return authToken;
        }
    }

    public class OrderService : IOrderService
    {
        public AuthToken GenerateToken(AuthUser user)
        {
            var auth = "H98HF9QD8HF928H9F8H293H89qh9hf9ahf98hH89=";
            var enc = Encoding.ASCII.GetBytes(auth);
            var key = new SymmetricSecurityKey(enc);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expirationDate = DateTime.UtcNow.AddHours(2);
            var tokenTest = new JwtSecurityToken(
                null,
                null,
                null,
                expires: expirationDate,
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenTest);
            var authToken = new AuthToken();
            authToken.Token = token;
            authToken.ExpirationDate = expirationDate;

            return authToken;
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
