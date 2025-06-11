using BookStoreCatalog_API.Models.Settings;
using System.Linq.Expressions;

namespace BookStoreCatalog_API.Repository.IRepository
{
    public interface IRepository<T> where T : class 
    {
        Task Create(T entity);

        Task<List<T>> GetAll(Expression<Func<T,bool>>? filter = null, string? includeProperties=null);

        Task<T> GetBook(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null);

        Task Remove(T entity);

        Task Save();

        PagedList<T> GetAllPaged(Parameters parameters, Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

    }
}
