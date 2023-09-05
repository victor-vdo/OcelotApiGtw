using OcelotApiGtw.Domain.Models;

namespace OcelotApiGtw.Domain.Interfaces
{
    public interface IPaymentService
    {
        AuthToken GenerateToken(AuthUser user);
    }
}
