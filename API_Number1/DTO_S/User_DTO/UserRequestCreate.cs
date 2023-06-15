namespace API_Number1.DTO_S.User_DTO
{
    public class UserRequestCreate
    {       
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; internal set; }
        public Guid CategoryId { get; internal set; }

        public UserRequestCreate(string name, string email,string Password,Guid CategoryId)
        {
            Name = name;
            Email = email;
            this.Password = Password;
            this.CategoryId = CategoryId;
        }

     
    }
}
