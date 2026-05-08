using ComChienMaDui.Models;

namespace ComChienMaDui.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
