using System.Linq.Expressions;

namespace DAL.Interfaces;

public interface IRepositoryBase<T> where T : class
{
    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);

    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task CreateAndSaveAsync(T entity);
    Task UpdateAndSaveAsync(T entity);
    Task DeleteAndSaveAsync(T entity);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}