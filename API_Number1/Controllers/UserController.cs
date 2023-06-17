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

namespace API_Number1.Controllers
{

    [ApiController]
    [Route("/[controller]")]
    public class UserController : ControllerBase
    {
        protected IServiceBase _factoryBase { get; set; }
        protected IUserRepository _UserRepository { get; set; }
        protected IPasswordHasher _passwordHasher { get; set; }
        protected IJwtService _jwtService { get; set; }
        protected ApplicationDbContext DbContext { get; set; }
        protected ILogger<User> logger1 { get; set; }
        public UserController(IServiceBase factoryBase, ApplicationDbContext context, ILogger<User> logger)
        {
            _factoryBase = factoryBase;
            _UserRepository = _factoryBase.CreateUserInterface();
            _passwordHasher = _factoryBase.CreateAuthenticationUserInterface();
            _jwtService = _factoryBase.CreateJwtService();
            logger1 = logger;
            DbContext = context;
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("{userId}/User")]
        public async Task<IResult> GetUserById([FromRoute] Guid userId)
        {
            var entity = await _UserRepository.GetEntityById(userId);
            UserResponse userResponse = (UserResponse)entity;
            return Results.Ok(userResponse);

        }
        [Authorize]
        [HttpGet]
        [Route("{userName}/Users")]
        public async Task<IResult> GetUserByName([FromRoute] string userName)
        {
            var entity = await _UserRepository.GetEntityByName(userName);
            UserResponse userResponse = (UserResponse)entity;
            return Results.Ok(userResponse);
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("Users")]
        public async Task<IResult> GetAllUsers()
        {
            var UsersList = await _UserRepository.GetAllEntities();
            List<UserResponse> ListResponse = new List<UserResponse>();
            foreach (var user in UsersList)
            {
                var UserResponse = (UserResponse)user;
                ListResponse.Add(UserResponse);
            };
            return Results.Ok(ListResponse);

        }

        [HttpPost]
        [Route("User")]
        public async Task<IResult> CreateUser([FromBody] UserRequestCreate userRequest)
        {
            var passwordHash = _passwordHasher.HashPassword(userRequest.Password, out var salt);
            var user = new User()
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                CategoryId = userRequest.CategoryId,
                PasswordHash = passwordHash,
                PasswordSalt = salt

            };
            await _UserRepository.CreateEntity(user);
            DbContext.SaveChanges();

            return Results.CreatedAtRoute("User", "Criado com sucesso");

        }
        [HttpPost]
        [Route("Login")]
        public async Task<IResult> LoginUser([FromBody] SignIn @in)
        {
            throw new Exception("Teste");
            var user = await _UserRepository.GetEntityById(@in.Id);
            var result = _passwordHasher.VerifyPassword(@in.Password, user.PasswordHash, user.PasswordSalt);
            if (!result)
            {
                return Results.BadRequest("Senha incorreta");
            }
            var jwt=_jwtService.GenerateToken(user);
            return Results.Ok(jwt);
        }
        [Authorize(Roles = "Administrator")]
        [HttpPut]//Acho que só o admin iria acessar o put, já que o user não conseguiria alterar todas as informações, tipo categoryId, etc...       
        [Route("{UserId}/User")]
        public async Task<IResult> UpdateEntity(Guid UserId, UserEditRequest userRequest)
        {


            var password = _passwordHasher.HashPassword(userRequest.Password, out var salt);
            var user = new User
            {
                Name = userRequest.Name,
                Email = userRequest.Email,
                CategoryId = userRequest.CategoryId,
                PasswordHash = password,
                PasswordSalt = salt

            };
            
            await _UserRepository.UpdateEntity(UserId, user);
            return Results.Ok(user);
        }
        [Authorize]
        [HttpPatch]
        [Route("/User")]
        public async Task<IResult> EditEntity([FromBody] JsonPatchDocument<User> jsonPatchDocument)
        {
            //Obtendo o token jwt - Aquele replacer é porque só quero o JWT, logo retirei o Bearer que vem junto
            var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //Manipular o jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            //Lendo o jwt em si, no caso o payload será lido também
            var JwtInfo = tokenHandler.ReadJwtToken(jwt);

            var UserIdClaim =  JwtInfo.Claims.FirstOrDefault(c => c.Type =="nameid");
            //Lembre-se que o claim tem a Key e Value, logo passar somente UserIdClaim não funciona
            var UserId = Guid.Parse(UserIdClaim.Value);

            //Dentro do método terá todas as verificaçãoes necessárias
            var entity = (UserResponse)await _UserRepository.UpdateEntityProperties(UserId, jsonPatchDocument);

            return Results.Ok("/User");
        }
    }
}
