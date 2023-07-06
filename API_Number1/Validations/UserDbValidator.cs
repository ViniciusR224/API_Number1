using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.ValidationInterfaces;
using API_Number1.Models;
using API_Number1.Repositories.Validation_Repository;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API_Number1.Validations
{
    public class UserDbValidator:AbstractValidator<SignUpRequest>
    {        
        private  IValidationRepository<User> validationRepository;
        public UserDbValidator(IValidationRepository<User> validationRepository)
        {            
            this.validationRepository = validationRepository;
             
            RuleFor(obj=>obj.Name).MustAsync(NameNotInUse).WithMessage("Nome já em uso, escolha outro.");
            RuleFor(obj => obj.Email).MustAsync(EmailNotInUse).WithMessage("Email já em uso, escolha outro");
        }
        private async Task<bool> NameNotInUse(string Name,CancellationToken token)
        {
            return !(await validationRepository.Exists(obj=>obj.Name == Name));
        }
        private async Task<bool> EmailNotInUse(string Email,CancellationToken token)
        {
            return !(await validationRepository.Exists(obj=>obj.Email == Email));
        }



    }
}
