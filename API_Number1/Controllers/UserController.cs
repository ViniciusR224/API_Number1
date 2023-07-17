using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IJwt_Service;
using API_Number1.Interfaces.IPassword_Hasher;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Interfaces.IService_Base;

using API_Number1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Cors;
using FluentValidation;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel.DataAnnotations;
using MySqlX.XDevAPI.Common;
using FluentValidation.Results;
using API_Number1.Interfaces.IAuthenticationProcess;
using API_Number1.Interfaces.IADM_Service;
using API_Number1.Interfaces.IUserService;
using Microsoft.AspNetCore.JsonPatch.Operations;
using API_Number1.Validations;

namespace API_Number1.Controllers
{

    [ApiController]//Adiciona o binding e validação do modelstate automáticos,logo você não precisa especificar [FromBody] [FromRoute] etc...
    [Route("/[controller]")]
    public class UserController : ControllerBase
    {       
        protected ILogger<User> logger1 { get; set; }      
        protected IUser_Service _User_Service { get; }
        protected IAdm_Service adm_Service;
        public IValidator<SignUpRequest> SignUpValidator { get; }      
        protected IUserPatchValidation _PatchValidation { get; }
        

        public UserController(ILogger<User> logger,IValidator<SignUpRequest> validator,IUser_Service user_Service,IUserPatchValidation userPatchValidation)
        {
            logger1 = logger;
            SignUpValidator = validator;
            _User_Service = user_Service;
            _PatchValidation = userPatchValidation;                   
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("{userId}/User")]
        public async Task<IResult> GetUserById(Guid userId)
        {
            var entity=await adm_Service.GetUserById(userId);
            UserResponse userResponse = (UserResponse)entity;
            return Results.Ok(userResponse);
        }
        [Authorize]
        [HttpGet]
        [Route("{userName}/Users")]
        public async Task<IResult> GetUserByName(string userName)
        {
            var entity = await _User_Service.GetUserByName(userName);
            UserResponse userResponse = (UserResponse)entity;
            return Results.Ok(userResponse);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("Users")]
        public async Task<IResult> GetAllUsers()
        {
            var UsersList = await adm_Service.GetAllUsers();
            List<UserResponse> ListResponse = new List<UserResponse>();
            foreach (var user in UsersList)
            {
                var UserResponse = (UserResponse)user;
                ListResponse.Add(UserResponse);
            };
            return Results.Ok(ListResponse);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("User")]
        public async Task<IResult> CreateUserEntity(SignUpRequest userRequest)
        {
            
            //Lembrar de refatorar e adicionar em um filtro a questão das validações
            var ValidationResult = await SignUpValidator.ValidateAsync(userRequest);
            if (!ValidationResult.IsValid)
            {
                return ValidationProblems(ValidationResult);
            }
            var entity = await _User_Service.CreateUserEntity(userRequest);
            return Results.Ok("Criado com sucesso");

        }
        private IResult ValidationProblems(FluentValidation.Results.ValidationResult validationResult)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(),
                    "A validação falhou por não seguir as regras",
                    "SignUp", 400,
                    "Falha na validação",
                    "Error");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IResult> LoginUser(SignInRequest @in)
        {
            var Result= await _User_Service.LoginUser(@in);
            return Result;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]//Acho que só o admin iria acessar o put, já que o user não conseguiria alterar todas as informações, tipo categoryId, etc...       
        [Route("{UserId}/User")]
        public async Task<IResult> UpdateEntity(Guid UserId, ModifyingUserRequest userRequest)
        {
            var entity=await adm_Service.UpdateEntity(UserId, userRequest);
            return Results.Ok(entity);
        }

        //[Authorize]
        [HttpPatch]
        [Route("/User")]
        public async Task<IResult> EditEntity(JsonPatchDocument<User> jsonPatchDocument)
        {
            var result=_PatchValidation.PatchResultProcess(jsonPatchDocument);
            if (!result.IsValid)
            {
                return Results.BadRequest(result._errorsValidation);
            }
           

            var UserId = GetUserIdInJwt();
            //Dentro do método terá todas as verificaçãoes necessárias
            var EditResult = await _User_Service.EditEntity(UserId, jsonPatchDocument);

            return EditResult;
        }
        [Authorize]
        [HttpDelete]
        [Route("Resources")]
        public async Task<IResult> DeleteEntityById()
        {
            //Pegar um claim especifico no jwt
            var UserId= GetUserIdInJwt();
            return await _User_Service.DeleteEntity(UserId);
        }
        private Guid GetUserIdInJwt()
        {
            //Colocar dentro do Service ou em um método no controller, por enquanto pelo menos

            //Obtendo o token jwt - Aquele replacer é porque só quero o JWT, logo irei retirar o "Bearer" que vem junto
            var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            //Manipular o jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            //Lendo o jwt em si, no caso o payload será lido também
            var JwtInfo = tokenHandler.ReadJwtToken(jwt);
            
            var UserIdClaim = JwtInfo.Claims.FirstOrDefault(c => c.Type == "nameid");

            //Lembre-se que o claim tem a Key e Value, logo passar somente UserIdClaim não funciona

            var UserId = Guid.Parse(UserIdClaim.Value);
            return UserId;
        }
    }
}
