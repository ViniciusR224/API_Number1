using API_Number1.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Net.Http.Headers;
using System.Data;

namespace API_Number1.Validations;



public interface IUserPatchValidation
{
    ValidationResult ValidationProcess(JsonPatchDocument<User> jsonPatchDocument);
}


//Quando é coisas compelexas assim, faça a classe que verifica todas as caracteristicas para ver se tem necessário para depois definir o que validar
//E então monte os validadores a parte, já que em situações assim eu não sei o que irá chegar aqui, e tem muitas coisas que podem vir
public class UserPatchValidation2 : IUserPatchValidation
{
    private List<string> CaminhosValidos = new List<string> { "Name", "Email", "Password" };
    private ValidationResult ValidationResults = new ValidationResult();
    public ValidationResult ValidationProcess(JsonPatchDocument<User> jsonPatchDocument)
    {
        if (jsonPatchDocument.Operations.Count > 3)
        {
            ValidationResults.Errors.Add(new ValidationFailure("", "Número de operações não permitido"));
        }

        foreach (var operation in jsonPatchDocument.Operations)
        {
            if (operation.op != OperationType.Replace.ToString().ToLower()
                && !CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {

                ValidationResults.Errors.Add(new ValidationFailure($"Path:{operation.path} e OP Inválidos:{operation.op}", "Coloque um caminho e um tipo Operação válidos"));
            }
            else if (operation.op != OperationType.Replace.ToString().ToLower()
                && CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {
                ValidationResults.Errors.Add(new ValidationFailure($"Op inválido:{operation.op}", "Coloque uma Op válida"));
            }
            else if (operation.op == OperationType.Replace.ToString().ToLower()
                && !CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {
                ValidationResults.Errors.Add(new ValidationFailure($"Path inválido:{operation.path}", "Coloque um path válido"));
            }

        }
        if (ValidationResults.Errors.Count > 0)
        {
            return ValidationResults;
        }
        foreach (var operation in jsonPatchDocument.Operations)
        {
            switch (operation.path)
            {
                case "Name":
                    ValidationResults.Errors.AddRange(new NameValidator().Validate(operation).Errors);
                    continue;
                case "Email":
                    ValidationResults.Errors.AddRange(new EmailValidator().Validate(operation).Errors);
                    continue;
                case "Password":
                    ValidationResults.Errors.AddRange(new PasswordValidator().Validate(operation).Errors);
                    continue;
                default:
                    break;
            }
        }
        return ValidationResults;


    }
}
public class PasswordValidator : AbstractValidator<Operation<User>>
{
    public PasswordValidator()
    {
        RuleFor(operation => operation.value).NotEmpty().NotNull();
    }
}
public class EmailValidator : AbstractValidator<Operation<User>>
{
    public EmailValidator()
    {
        RuleFor(operation => operation.value).NotEmpty().NotNull();
    }
}
public class NameValidator:AbstractValidator<Operation<User>>
{
    public NameValidator()
    {
        RuleFor(operation=>operation.value).NotNull().NotEmpty().WithName("Nome");
    }
}
