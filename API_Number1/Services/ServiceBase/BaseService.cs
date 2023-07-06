
using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.IUser_Repository;

namespace API_Number1.Services.FactoryBase
{

    public class BaseService : IServiceBase
    {
        //Retirado após refatoração de controller e serviços,processos e etc...


        public IServiceProvider _serviceProvider { get; set; }
        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IUserRepository CreateUserRepository()
        {
            return _serviceProvider.GetService<IUserRepository>() ?? throw new NotImplementedException();
        }

        public IPasswordHasher CreateAuthenticationUserInterface()
        {
            return _serviceProvider.GetService<IPasswordHasher>() ?? throw new NotImplementedException();
        }

        public IJwtService CreateJwtService()
        {
            return _serviceProvider.GetService<IJwtService>() ?? throw new NotImplementedException();
        }

        public IAuthentication_Process CreateAuthenticationProcess()
        {
            return _serviceProvider.GetService<IAuthentication_Process>() ?? throw new NotImplementedException();
        }
    }
}
