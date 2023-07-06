using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.ISigUpProcess;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;

namespace API_Number1.Services.SignUpProcessService
{
    public class SignUpProcess : ISignUpProcess
    {
        protected readonly IPasswordHasher _PasswordHasher;
        protected readonly IUserRepository _UserRepository;
        public SignUpProcess(IPasswordHasher passwordHasher,IUserRepository userRepository)
        {
            _PasswordHasher = passwordHasher;
            _UserRepository = userRepository;
        }
        public async Task<IResult> CreateUserProcess(SignUpRequest signUpRequest)
        {
            var passwordHash = _PasswordHasher.HashPassword(signUpRequest.Password, out var salt);
            var user = new User()
            {
                Name = signUpRequest.Name,
                Email = signUpRequest.Email,
                CategoryId = signUpRequest.CategoryId,
                PasswordHash = passwordHash,
                PasswordSalt = salt

            };
            await _UserRepository.CreateEntity(user);
            return Results.Created("/User","Criado com sucesso");
        }
    }
}
