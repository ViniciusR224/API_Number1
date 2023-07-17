using API_Number1.DTO_S.User_DTO;
using API_Number1.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace API_Number1.Validations;
//Acho que quando é coisas complexas assim é melhor dividir tudo em processos que verificam as propriedades separadamente 
//Como aqui era algo que cada path tinha sua lista de ValidationFailure do value, essa foi a melhor solução para isso tudo
//Depois foi só retornar algo que tinha o resultado da validação e então uma lista desse objeto complexo, dai evito de misturar tudo
//Porque cada não teria aquela mistura que tinha antes, aqui cada error tem path e sua lista própria de validationFailures,
//E tudo dividido certinho, fornique sim.
public interface IUserPatchValidation
{
    ValidationPatchResult PatchResultProcess(JsonPatchDocument<User> jsonPatchDocument);
    
}
public class UserPatchValidation2 : IUserPatchValidation
{
    private List<string> CaminhosValidos = new List<string> { "Name", "Email", "Password" };   
    public ValidationPatchResult PatchResultProcess(JsonPatchDocument<User> jsonPatchDocument)
    {
        var validationErrors = ValidationProcess(jsonPatchDocument);
        if(validationErrors.Count > 0)
        {
            var FailureResult = new ValidationPatchResult()
            {
                IsValid = false,
                _errorsValidation= validationErrors
            };
            return FailureResult;
        }
        var sucessResult= new ValidationPatchResult()
        {
            IsValid = true
            
        };
        return sucessResult;
    }

    private List<Error> ValidationProcess(JsonPatchDocument<User> jsonPatchDocument)
    {
        var _errorsValidation = new List<Error>();
        if (jsonPatchDocument.Operations.Count > 3)
        {
            var error=new Error()
            {
                Path="JsonPatchDocument",
                _validationFailures=new List<ValidationFailure> { new ValidationFailure("","Número de propriedades não permitida")}
            };
            _errorsValidation.Add(error);          
            return _errorsValidation;
        }
        
        foreach (var operation in jsonPatchDocument.Operations)
        {
            if (operation.op != OperationType.Replace.ToString().ToLower()
                && !CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {
                var failure = new ValidationFailure($"Path:{operation.path} e OP Inválidos:{operation.op}", "Coloque um caminho e um tipo Operação válidos");
                var Errors = new Error()
                {
                    Path = $"{operation.path}",
                    _validationFailures = new List<ValidationFailure> { failure }
                };
                _errorsValidation.Add(Errors);   
            }
            else if (operation.op != OperationType.Replace.ToString().ToLower()
                && CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {
                var failure=new ValidationFailure($"Op inválido:{operation.op}", "Coloque uma Op válida");
                var Errors = new Error()
                {
                    Path = $"{operation.path}",
                    _validationFailures = new List<ValidationFailure> { failure }
                };
                _errorsValidation.Add(Errors);
            }
            else if (operation.op == OperationType.Replace.ToString().ToLower()
                && !CaminhosValidos.Any(ValidPath => ValidPath.Equals(operation.path, StringComparison.OrdinalIgnoreCase)))
            {
                var failure=new ValidationFailure($"Path inválido:{operation.path}", "Coloque um path válido");
                var Errors = new Error()
                {
                    Path = $"{operation.path}",
                    _validationFailures = new List<ValidationFailure> { failure }
                };
                _errorsValidation.Add(Errors);

            }

        }
        if (_errorsValidation.Count > 0) 
        {
           return _errorsValidation;
        }
        foreach (var operation in jsonPatchDocument.Operations)
        {
            switch (operation.path)
            {
                case "Name":                   
                    var NameErrors=new NameValidator().Validate(operation.value.ToString()).Errors;
                    if(NameErrors.Count > 0)
                    {                        
                        var Error = new Error()
                        {
                            Path = operation.path,
                            _validationFailures = NameErrors
                        };
                        _errorsValidation.Add(Error);

                    }                       
                    continue;
                case "Email":
                    var EmailErrors = new EmailValidator().Validate(operation.value.ToString()).Errors;
                    if (EmailErrors.Count > 0)
                    {
                        var Error = new Error()
                        {
                            Path = operation.path,
                            _validationFailures = EmailErrors
                        };
                        _errorsValidation.Add(Error);
                    }
                    continue;
                case "Password":
                    var PasswordErrors = new NameValidator().Validate(operation.value.ToString()).Errors;
                    if (PasswordErrors.Count > 0)
                    {
                        var Error = new Error()
                        {
                            Path = operation.path,
                            _validationFailures = PasswordErrors
                        };
                        _errorsValidation.Add(Error);
                    }
                    continue;
                default:
                    break;
            }
        }
        return _errorsValidation;


    }
}
public class ValidationPatchResult
{
    public bool IsValid { get; set; }
    public List<Error> _errorsValidation = new List<Error>();
}
public class Error
{
    public string Path { get; set; }
    public List<ValidationFailure> _validationFailures = new List<ValidationFailure>();
    
    
}
public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(operation => operation).NotEmpty().NotNull();
    }
}
public class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(operation => operation).NotEmpty().NotNull();
    }
}
public class NameValidator:AbstractValidator<string>
{
    public NameValidator()
    {
        RuleFor(operation=>operation).NotNull().NotEmpty().Length(10,11).WithName("Nome");
    }
}
