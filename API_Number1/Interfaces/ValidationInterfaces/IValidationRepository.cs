using System.Linq.Expressions;

namespace API_Number1.Interfaces.ValidationInterfaces
{
    public interface IValidationRepository<T> where T : class
    {
        Task<bool> Exists(Expression<Func<T, bool>> expression);

    }
}
