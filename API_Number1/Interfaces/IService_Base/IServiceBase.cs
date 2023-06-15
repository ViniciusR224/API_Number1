using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IUser_Repository;

namespace API_Number1.Interfaces.IService_Base
{
    public interface IServiceBase
    {
        IServiceProvider _serviceProvider { get; set; }
        IUserRepository CreateUserInterface();
        IPasswordHasher CreateAuthenticationUserInterface();
        IJwtService CreateJwtService();

    }
}
