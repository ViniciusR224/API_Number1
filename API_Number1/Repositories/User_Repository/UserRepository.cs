using API_Number1.AppDbContext;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Interfaces.IUser_Repository;
using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

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

        public async Task<User> CreateEntity(User user)
        {
           
            var NewEntity = await RepostoryBase.CreateEntity(user);
            await applicationDbContext.SaveChangesAsync();
            return NewEntity;
            
        }

        public async Task DeleteEntity(Guid id)
        {
            await RepostoryBase.DeleteEntity(id);
            await applicationDbContext.SaveChangesAsync();
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
            await applicationDbContext.SaveChangesAsync();
            return NewEntity;
        }

        public async Task<User> UpdateEntityProperties(Guid id, JsonPatchDocument<User> jsonPatchDocument)
        {
            var user = await RepostoryBase.GetEntityById(id);
            jsonPatchDocument.ApplyTo(user);
            var NewEntity = await RepostoryBase.UpdateEntity(id, user);
            await applicationDbContext.SaveChangesAsync();
            return NewEntity;
        }      
        public async Task<User> GetUserByEmail(string email)
        {
            var entity=await RepostoryBase.GetEntityByEmail(email);           
            return entity;
        }
    }
}
