
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.IUser_Repository;

namespace API_Number1.Services.FactoryBase
{
    public class BaseService : IServiceBase
    {
        public IServiceProvider _serviceProvider { get; set; }
        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IUserRepository CreateUserInterface()
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
    }
}
