using API_Number1.Models;


namespace API_Number1.DTO_S.User_DTO
{
    public class UserResponse
    {
        public string Name { get; set; }
        public string Email { get; set; }

        
        public static explicit operator UserResponse(User user)
        {
            return new UserResponse { Name = user.Name, Email = user.Email };
            
        }
    }
}
