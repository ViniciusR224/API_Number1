using API_Number1.AppDbContext;
using API_Number1.DTO_S.User_DTO;
using API_Number1.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace API_Number1.Interfaces.IUser_Repository
{
    public interface IUserRepository
    {
        ApplicationDbContext applicationDbContext { get; set; }
        //GETs
        /// <summary>
        /// Obtém uma entidade pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade a ser obtida.</param>
        /// <returns>A entidade obtida.</returns>
        Task<User> GetEntityById(Guid id);

        /// <summary>
        /// Obtém uma entidade pelo seu nome.
        /// </summary>
        /// <param name="name">O nome da entidade a ser obtida.</param>
        /// <returns>A entidade obtida.</returns>
        Task<User> GetEntityByName(string name);

        /// <summary>
        /// Obtém todas as entidades.
        /// </summary>
        /// <returns>Todas as entidades obtidas.</returns>
        Task<IEnumerable<User>> GetAllEntities();

        //POSTs
        /// <summary>
        /// Cria uma nova entidade.
        /// </summary>
        /// <param name="entity">A entidade a ser criada.</param>
        /// <returns>A entidade criada.</returns>
        Task<User> CreateEntity(User entity);

        //PUTs
        /// <summary>
        /// Atualiza uma entidade pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da entidade a ser atualizada.</param>
        /// <param name="entity">A entidade atualizada.</param>
        /// <returns>A entidade atualizada.</returns>
        Task<User> UpdateEntity(Guid id, User entity);

        //PATCHs

        Task<User> UpdateEntityProperties(Guid id, JsonPatchDocument<User> jsonPatchDocument);

        //DELETE
        /// <summary>
        /// Exclui uma entidade com o ID especificado.
        /// </summary>
        /// <param name="id">O ID da entidade a ser excluída.</param>
        /// <returns>A tarefa que representa a conclusão da operação.</returns>
        Task DeleteEntity(Guid id);
        

    }
}
