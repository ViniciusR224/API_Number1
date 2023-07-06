using API_Number1.AppDbContext;
using API_Number1.Interfaces.IRepository_Base;
using API_Number1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace API_Number1.Repositories.Repository_Base
{
    public class RepositoryBase<TEntity> : IRepostoryBase<TEntity> where TEntity : Entity
    {
        protected readonly ApplicationDbContext _applicationDbContext;
        protected readonly DbSet<TEntity> _entities;

        public RepositoryBase(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _entities = applicationDbContext.Set<TEntity>();
        }

        
        public async Task<TEntity> CreateEntity(TEntity entity)
        {
            await _entities.AddAsync(entity);  
            return entity;
        }

        public async Task DeleteEntity(Guid id)
        {

            var entity = await _entities.FindAsync(id);
            _entities.Remove(entity);
                
            
        }

        public async Task<IEnumerable<TEntity>> GetAllEntities()
        {
            var AllEntitys = await _entities.AsNoTracking().ToListAsync();
            return AllEntitys;
        }

        public async Task<TEntity> GetEntityByEmail(string email)
        {
            var entity=await _entities.FindAsync(email);
            return entity;
        }

        public async Task<TEntity> GetEntityById(Guid id)
        {
            var entity=await _entities.FindAsync(id);
            return entity;
        }

        public async Task<TEntity> GetEntityByName(string name)
        {
            var entity = await _entities.FirstOrDefaultAsync(obj => obj.Name ==name);
            return entity;
        }

        public async Task<TEntity> UpdateEntity(Guid id, TEntity NewEntity)
        {
            var entity = await _entities.FindAsync(id);
            _entities.Entry(entity).CurrentValues.SetValues(NewEntity);
            return NewEntity;
        }
        


    }
}
