using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IPatchProcess;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;
using API_Number1.Repositories.User_Repository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MySqlX.XDevAPI.CRUD;

namespace API_Number1.Services.Patch_Process;

public class PatchProcess : IPatch_Process
{
    private List<string> CaminhosValidos = new List<string> { "Name", "Email","Password" };        
    private IPasswordHasher _passwordHasher;
    protected IUserRepository _userRepository;
    public PatchProcess(IPasswordHasher passwordHasher, IUserRepository userRepository)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
    }
    public async Task<IResult> UserPatchProcess(Guid id,JsonPatchDocument<User> jsonPatchDocument)
    {

        if (jsonPatchDocument.Operations.Count > 3)
            return Results.BadRequest("Quantidade de operações não autorizada");

        //Criando uma lista que aceita Operation em si - Se tiver duvidas de como é essa operation vá para a "Definição. 
        //public List<Operation<TModel>> Operations { get; private set; }. Lembre que o JsonPatchDocument tem essa propriedade de List, 
        //logo você pode fazer coisas assim
        List<Operation<User>> RemoveOperations = new List<Operation<User>>();
       
        foreach (var operation in jsonPatchDocument.Operations)
        {
            //Fazendo as verificãções das operations permitidas a fazer troca, no caso limitaremos ela aqui, teoricamente em uma situação em que
            //o client envie para atualizar o Id ou PasswordHash por exemplo, tal coisa não ocorreria. 
            var path = operation.path.ToString();//Utilizar o ! não seria o certo já que o "Name" aqui não daria match com o "Email" e entraria no if e removeria a operation
            //if (Regex.IsMatch(path, "Name") || Regex.IsMatch(path, "Email", RegexOptions.IgnoreCase))
            //{
            //    continue;
            //}
            //RemoveOperations.Add(operation);

            //Essa abordagem talvez seja melhor, utilizo o Any para verificar cada obj da list
            if (CaminhosValidos.Any(PathValido => PathValido== operation.path))
            {
                continue;
            }
            RemoveOperations.Add(operation);
        }
        foreach (var operationToRemove in RemoveOperations)
        {
            jsonPatchDocument.Operations.Remove(operationToRemove);
        }

        if (jsonPatchDocument.Operations.Count == 0)
            return Results.BadRequest("Nenhuma Operação válida");
        

        //Patch da senha, se eu adcionar, vou ter que mudar tudo e criar um processo nas regras de negócio
        var SenhaOperation = jsonPatchDocument.Operations.Find(op => op.path.ToString() == "Password");
        if(SenhaOperation!=null)
        {
            var NewPassword = SenhaOperation.value.ToString();
            jsonPatchDocument.Operations.Remove(SenhaOperation);
            var NewHashPassword = _passwordHasher.HashPassword(NewPassword, out string salt);
            var PasswordHashOperation = new Operation<User>
            {
                path = "PasswordHash",
                op = "replace",
                value = NewHashPassword
            };
            var PasswordSaltOperation = new Operation<User>
            {
                path = "PasswordSalt",
                op = "replace",
                value = salt
            };
            jsonPatchDocument.Operations.Add(PasswordHashOperation);
            jsonPatchDocument.Operations.Add(PasswordSaltOperation);
        }
        
        await _userRepository.UpdateEntityProperties(id, jsonPatchDocument);
        return Results.Ok("Usuário Atualizado");
    }
}
