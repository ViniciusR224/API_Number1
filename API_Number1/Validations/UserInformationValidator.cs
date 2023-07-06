using API_Number1.DTO_S.User_DTO;
using FluentValidation;
using System.Text.RegularExpressions;

namespace API_Number1.Validations
{
    public class UserInformationValidator:AbstractValidator<SignUpRequest>
    {
        public UserInformationValidator()
        {
            RuleFor(obj => obj.Name)
                .NotNull()
                .NotEmpty()
                .Length(5, 10);
            
            RuleFor(obj => obj.Email)
                .NotEmpty()
                .NotNull()
                .Must(ValidEmail).WithMessage("Email Inválido");
            
            RuleFor(obj => obj.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(5).WithMessage("Seu {PropertyName} deve possuir ao menos 5 caracteres")               
                .Must(AtLeastOneNumber).WithMessage("Seu {PropertyName} deve possuir ao menos 1 número")
                .Must(OneUpperChar).WithMessage("Seu {PropertyName} deve possuir ao menos 1 caractere maisculo")
                .Must(ValidPassword).WithMessage("Somente letras e números são permitidos");
                
        }
        private bool ValidEmail(string email)
        {
            return Regex.IsMatch(email, "^[A-Za-z0-9]+([.][A-Za-z0-9]+)?@([a-z])+.[a-z]{2,3}$");
        }
        private bool AtLeastOneNumber(string password)
        {
            return Regex.IsMatch(password, "[0-9]");
        }
        private bool OneUpperChar(string password)
        {
            return Regex.IsMatch(password, "[A-Z]");
        }
        private bool ValidPassword(string password)
        {
            return Regex.IsMatch(password, "^[a-zA-Z0-9]+$");
        }
    }
}
