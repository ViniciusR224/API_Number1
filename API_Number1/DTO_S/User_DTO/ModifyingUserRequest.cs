namespace API_Number1.DTO_S.User_DTO
{
    public class ModifyingUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }


    }
}
