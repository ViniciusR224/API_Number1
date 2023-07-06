using API_Number1.AppDbContext;
using API_Number1.Interfaces.ValidationInterfaces;
using API_Number1.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_Number1.Repositories.Validation_Repository
{
    public class ValidationRepository<T> : IValidationRepository<T> where T : class
    {
        public ValidationRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _entities=dbContext.Set<T>();    
        }

        public ApplicationDbContext DbContext { get; }
        protected DbSet<T> _entities { get;set; }
        //Consegui entender, é basicamente uma expressão lambda encapsulada de um Func, lembre se que o func tem uma entrada e uma saida
        //Se tivesse aqui func<T,User> Teriamos uma expressão lambda que pode fazer comparações ou qualquer coisa de um tipo T e retorna um objeto.
        // por exemplo, se quisessemos comparar uma propriedade ou objeto em si com um outro, isso nos retornaria o user,veja que AnyAsync aceita Expression de func
        //Seria só mudar o retorno, já que ele só esta vendo se o objeto existe no banco de dados _entities no caso
        public async Task<bool> Exists(Expression<Func<T, bool>> expression)
        {
            return await _entities.AnyAsync(expression);
        }
    }
}