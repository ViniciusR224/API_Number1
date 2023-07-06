using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text.RegularExpressions;

namespace API_Number1.Services.AuthenticationProcessService
{
    public class AuthenticationProcess : IAuthentication_Process
    {
        protected readonly IUserRepository _UserRepository;
        protected readonly IJwtService _jwtService;
        protected readonly IPasswordHasher _passwordHasher;
        public AuthenticationProcess(IServiceBase baseRepository)
        {
            _UserRepository=baseRepository.CreateUserRepository();
            _jwtService=baseRepository.CreateJwtService();
            _passwordHasher=baseRepository.CreateAuthenticationUserInterface();
        }
        public async Task<IResult> UserAuthenticationProcess(SignInRequest @in)
        {
            if (IsEmailIdentifier(@in.UserIdentifier))
            {
                var user = await _UserRepository.GetUserByEmail(@in.UserIdentifier);
                if (user == null)
                {                   
                    return Results.BadRequest("Email Inválido");
                }
                return await VerifyPassword(user, @in.Password);
            }
            else if (IsNameIdentifier(@in.UserIdentifier))
            {
                var user = await _UserRepository.GetEntityByName(@in.UserIdentifier);
                if (user == null)
                {
                    return Results.BadRequest("UserName Inválido");
                }
                return await VerifyPassword(user, @in.Password);
            }
            return Results.NotFound("UserName ou Email Inválidos - Tente novamente");
        }
        //Essa de passar o User diretamente foi uma boa, já que dentro do processo de obter o user eu tinha ele em si já presente com os dados
        //Lembre-se dessas coisas. 
        private async Task<IResult> VerifyPassword(User user,string SignInRequestPassword)
        {
            var result = _passwordHasher.VerifyPassword(SignInRequestPassword, user.PasswordHash,user.PasswordSalt);
            if (!result)
            {
                return Results.BadRequest("Senha incorreta");
            }
            var jwt = _jwtService.GenerateToken(user);
            return Results.Ok(jwt);
        }
        private bool IsNameIdentifier(string Name)
        {
            return Regex.IsMatch(Name, "^[a-zA-Z]+$");

        }
        private bool IsEmailIdentifier(string Email)
        {
            return Regex.IsMatch(Email, "^[A-Za-z0-9]+([.][A-Za-z0-9]+)?@([a-z])+.[a-z]{2,3}$");
        }
    }

}
