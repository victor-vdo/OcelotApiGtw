using OcelotApiGtw.Domain.Models;

namespace OcelotApiGtw.Domain.Interfaces
{
    public interface IOrderService
    {
        AuthToken GenerateToken(AuthUser user);
    }
}
