using System.Linq.Expressions;

namespace DAL.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    Task<List<T>> FindAllAsync(
        bool trackChanges = false, 
        CancellationToken cancellationToken = default);

    Task<List<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    Task<T?> FindFirstByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges = false,
        CancellationToken cancellationToken = default);

    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}