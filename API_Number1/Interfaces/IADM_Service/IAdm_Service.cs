using API_Number1.DTO_S.User_DTO;
using API_Number1.Models;

namespace API_Number1.Interfaces.IADM_Service
{
    public interface IAdm_Service
    {
        Task<User> GetUserById(Guid guid);
        
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> UpdateEntity(Guid UserId, ModifyingUserRequest userRequest);
    }
}
