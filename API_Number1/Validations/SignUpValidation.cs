using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.ValidationInterfaces;
using API_Number1.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API_Number1.Validations
{
    public class SignUpValidation:AbstractValidator<SignUpRequest>
    {
        public IValidationRepository<User> ValidationRepository { get; }
        public SignUpValidation(IValidationRepository<User> validationRepository)
        {
            ValidationRepository = validationRepository;

            RuleFor(obj => obj).SetValidator(new UserInformationValidator()).DependentRules(() =>
            {
                RuleFor(obj => obj).SetValidator(new UserDbValidator(validationRepository));
            });            
        }

    }
}
