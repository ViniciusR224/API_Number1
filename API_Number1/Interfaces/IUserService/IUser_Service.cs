using API_Number1.DTO_S.User_DTO;
using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace API_Number1.Interfaces.IUserService
{
    public interface IUser_Service
    {
        //ADM Role
        //Task<User> GetUserById(Guid guid);               
        //ADM
        //Task<IResult> GetAllUsers();
        Task<User> GetUserByName(string username);
        Task<IResult> CreateUserEntity(SignUpRequest userRequest);
        Task<IResult> LoginUser(SignInRequest @in);
        //ADM
        //Task<IResult> UpdateEntity(Guid UserId, ModifyingUserRequest userRequest);
        Task<IResult> EditEntity(Guid id,JsonPatchDocument<User> jsonPatchDocument);
        Task<IResult> DeleteEntity(Guid id);

    }
}
