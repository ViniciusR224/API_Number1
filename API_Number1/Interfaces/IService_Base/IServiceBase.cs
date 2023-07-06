using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IUser_Repository;

namespace API_Number1.Interfaces.IService_Base
{
    public interface IServiceBase
    {
        //Retirado após refatoração de controller e serviços,processos e etc...
        
        IServiceProvider _serviceProvider { get; set; }
        IUserRepository CreateUserRepository();
        IPasswordHasher CreateAuthenticationUserInterface();
        IJwtService CreateJwtService();
        IAuthentication_Process CreateAuthenticationProcess();
    }
}
