using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_Number1.DTO_S.User_DTO
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "O campo '{0}' não pode ser nulo ou vazio ")]

        public string UserIdentifier { get; set; } = string.Empty;
        [Required(ErrorMessage = "O campo {0} não pode ser nulo ou vazio")]
        public string Password { get; set; } = string.Empty;
    }
}
