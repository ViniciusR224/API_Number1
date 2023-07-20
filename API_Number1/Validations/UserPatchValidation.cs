using API_Number1.Interfaces.ValidationInterfaces;
using API_Number1.Models;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Text.RegularExpressions;
namespace API_Number1.Validations;

public class UserPatchValidation:AbstractValidator<List<Operation<User>>>
{
    public UserPatchValidation()
    {
        RuleForEach(list => list).SetValidator(new PatchValidation());


    }  
}
public class PatchValidation:AbstractValidator<Operation<User>> 
{
    public PatchValidation()
    {
        When(op => op.path == "Name", () =>
        {
            RuleFor(op => op.value.ToString())
            .NotEmpty().WithMessage("Nome não pode ser vazio")
            .NotNull().WithMessage("Nome não pode ser nulo")
            .MinimumLength(4).WithMessage("Nome deve possuir ao menos 4 caracteres")
            .MaximumLength(10)
            .WithName("Name");
        });
        When(op => op.path == "Email", () =>
        {
            RuleFor(op => op.value.ToString())
            .NotEmpty().WithMessage("Email não pode ser vazio")
            .NotNull().WithMessage("Email não pode ser nulo")
            .MinimumLength(4).WithMessage("Email deve possuir ao menos 4 caracteres")
            .MaximumLength(10)
            .WithName("Email");
        });


        //Assim não funciona, porque ele não aceita duas sobrecargas, e se eu tentasse fazer um Setvalidator junto do Ruleforeach
        //Ele iria acabar pegando resultados errados porque caso uma operation permitida não estivesse presente, iria dar 
        //Os erros de validação, o que eu não queria, já que o user escolheu não mudar, não que a validação dessa operação falhou...
        //RuleFor(op => op.path == "Name", () =>
        //{

        //});
    }
   
}

