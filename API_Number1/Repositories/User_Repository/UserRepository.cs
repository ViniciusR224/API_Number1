using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace API_Number1.Repositories.User_Repository
{
    public class UserRepository : IUserRepository
    {

        

        public ApplicationDbContext applicationDbContext { get; set; }
        public DbSet<User> Users { get; set; }
        protected IRepostoryBase<User> RepostoryBase { get; set; }

        public UserRepository(ApplicationDbContext applicationDbContext, IRepostoryBase<User> repostoryBase)
        {
            this.applicationDbContext = applicationDbContext;
            Users=applicationDbContext.Users;
            RepostoryBase=repostoryBase;
        }

        public async Task<User> CreateEntity(User entity)
        {
            
            var NewEntity = await RepostoryBase.CreateEntity(entity);
            return NewEntity;
            
        }

        public async Task DeleteEntity(Guid id)
        {
            await RepostoryBase.DeleteEntity(id);
            
        }

        public async Task<IEnumerable<User>> GetAllEntities()
        {
            var entities=await RepostoryBase.GetAllEntities();
            return entities;
        }

        public async Task<User> GetEntityById(Guid id)
        {
            var entity=await RepostoryBase.GetEntityById(id);           
            return entity;
        }

        public async Task<User> GetEntityByName(string name)
        {
            var entity=await RepostoryBase.GetEntityByName(name);
            return entity;
        }

        public async Task<User> UpdateEntity(Guid id, User user)
        {          
            var NewEntity =await RepostoryBase.UpdateEntity(id, user);
            return NewEntity;
        }

        public async Task<User> UpdateEntityProperties(Guid id, JsonPatchDocument<User> jsonPatchDocument)
        {
            //Criando uma lista que aceita Operation em si - Se tiver duvidas de como é essa operation vá para a "Definição. 
            //public List<Operation<TModel>> Operations { get; private set; }. Lembre que o JsonPatchDocument tem essa propriedade de List, 
            //logo você pode fazer coisas assim
            List<Microsoft.AspNetCore.JsonPatch.Operations.Operation<User>> RemoveOperations = new List<Microsoft.AspNetCore.JsonPatch.Operations.Operation<User>>();
            foreach (var operation in jsonPatchDocument.Operations)
            {
                //Fazendo as verificãções das operations permitidas a fazer troca, no caso limitaremos ela aqui, teoricamente em uma situação em que
                //o client envie para atualizar o Id ou PasswordHash por exemplo, tal coisa não ocorreria. 
                var path = operation.path.ToString();
                if (!Regex.IsMatch(path, "Name", RegexOptions.IgnoreCase) || !Regex.IsMatch(path, "Email", RegexOptions.IgnoreCase))
                {
                    RemoveOperations.Add(operation);
                }
            }
            foreach (var operationToRemove in RemoveOperations)
            {
                jsonPatchDocument.Operations.Remove(operationToRemove);
            }
            var user = await RepostoryBase.GetEntityById(id);
            jsonPatchDocument.ApplyTo(user);
            var NewEntity = await RepostoryBase.UpdateEntity(id, user);
            
            return NewEntity;
        }
    }
}
