using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IPatchProcess;
using API_Number1.Interfaces.IService_Base;
using API_Number1.Interfaces.ISigUpProcess;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Interfaces.IUserService;
using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace API_Number1.Services.User_Service
{
    public class UserService : IUser_Service
    {
                     
        protected readonly IAuthentication_Process _ProcessAuthentication;
        private readonly IUserRepository _userRepository;
        protected readonly ISignUpProcess _signUpProcess;
        protected readonly IPatch_Process _patch_Process;
        public UserService(ISignUpProcess signUpProcess,IAuthentication_Process authentication_Process,IUserRepository userRepository,IPatch_Process patch_Process)
        {
            _ProcessAuthentication = authentication_Process;
            _userRepository = userRepository;
            _signUpProcess = signUpProcess;
            _patch_Process = patch_Process;
        }
        public async Task<IResult> CreateUserEntity(SignUpRequest userRequest)
        {
            return await _signUpProcess.CreateUserProcess(userRequest);
        }
       

        public async Task<IResult> EditEntity(Guid Id,JsonPatchDocument<User> jsonPatchDocument)
        {
            var jsonPatch = _patch_Process.UserPatchProcess(jsonPatchDocument);
            var entity= (UserResponse)await _userRepository.UpdateEntityProperties(Id, jsonPatch);
            return Results.Ok($"{entity}" + "Atualizado com sucesso");
        }

        
        public async Task<IResult> LoginUser(SignInRequest @in)
        {
            return await _ProcessAuthentication.UserAuthenticationProcess(@in);
        }

        public async Task<User> GetUserByName(string username)
        {
            return await _userRepository.GetEntityByName(username);
        }

        public async Task<IResult> DeleteEntity(Guid id)
        {
            await _userRepository.DeleteEntity(id);
            return Results.Ok("Usuario deletado");
        }
    }
}
