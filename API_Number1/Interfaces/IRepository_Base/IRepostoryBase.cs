using API_Number1.Models;
using System.Linq.Expressions;

namespace API_Number1.Interfaces.IRepository_Base
{
    public interface IRepostoryBase<TEntity> where TEntity:Entity
    {
        //GETs
        /// <summary>
        /// Obtém uma entidade pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade a ser obtida.</param>
        /// <returns>A entidade obtida.</returns>
        Task<TEntity> GetEntityById(Guid id);

        /// <summary>
        /// Obtém uma entidade pelo seu nome.
        /// </summary>
        /// <param name="name">O nome da entidade a ser obtida.</param>
        /// <returns>A entidade obtida.</returns>
        Task<TEntity> GetEntityByName(string name);

        /// <summary>
        /// Obtém uma entidade pelo seu Email
        /// </summary>
        /// <param name="email">O email da entidade a ser obtida</param>
        /// <returns>A entidade obtida pelo email</returns>
        Task<TEntity> GetEntityByEmail(string email);

        /// <summary>
        /// Obtém todas as entidades.
        /// </summary>
        /// <returns>Todas as entidades obtidas.</returns>
        Task<IEnumerable<TEntity>> GetAllEntities();
        
        //POSTs
        /// <summary>
        /// Cria uma nova entidade.
        /// </summary>
        /// <param name="entity">A entidade a ser criada.</param>
        /// <returns>A entidade criada.</returns>
        Task<TEntity> CreateEntity(TEntity entity);

        //PUTs //PATCHs Aqui eu uso o _entities.Entry(entity).CurrentValues.SetValues(NewEntity) Só preciso fazer umas alterações no repo base
        /// <summary>
        /// Atualiza uma entidade pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade a ser atualizada.</param>
        /// <param name="entity">A entidade atualizada.</param>
        /// <returns>A entidade atualizada.</returns>
        Task<TEntity> UpdateEntity(Guid id, TEntity entity);
        //PATCHs
        
        
        //DELETE
        /// <summary>
        /// Exclui uma entidade com o ID especificado.
        /// </summary>
        /// <param name="id">O ID da entidade a ser excluída.</param>
        /// <returns>A tarefa que representa a conclusão da operação.</returns>
        Task DeleteEntity(Guid id);
        /// <summary>
        /// Verifica se uma 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        

    }
}
