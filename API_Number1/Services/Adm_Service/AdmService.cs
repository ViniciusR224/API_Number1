using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IADM_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;
using API_Number1.Repositories.User_Repository;

namespace API_Number1.Services.Adm_Service
{
    public class AdmService : IAdm_Service
    {
        public IUserRepository _UserRepository { get; }
        public IPasswordHasher _PasswordHasher { get; }

        public AdmService(IUserRepository userRepository,IPasswordHasher passwordHasher)
        {
            _UserRepository = userRepository;
            _PasswordHasher = passwordHasher;
        }

        public async Task<User> GetUserById(Guid guid)
        {
            return await _UserRepository.GetEntityById(guid);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
           return await _UserRepository.GetAllEntities();
        }

        public async Task<User> UpdateEntity(Guid UserId, ModifyingUserRequest userRequest)
        {
            var password = _PasswordHasher.HashPassword(userRequest.Password, out var salt);
            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                CategoryId = userRequest.CategoryId,
                PasswordHash = password,
                PasswordSalt = salt

            };
            return await _UserRepository.UpdateEntity(UserId, user);

        }
    }
}
