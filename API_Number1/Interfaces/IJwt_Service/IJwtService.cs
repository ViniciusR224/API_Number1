using API_Number1.Models;

namespace API_Number1.Interfaces.IJwt_Service
{
    public interface IJwtService
    {
        string GenerateToken(User user);



    }
}
