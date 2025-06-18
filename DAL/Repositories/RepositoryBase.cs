using DAL.Contexts;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected FitnessTrackerContext _context { get; set; }

    protected RepositoryBase(FitnessTrackerContext context)
    {
        _context = context;
    }

    public IQueryable<T> FindAll(bool trackChanges = false)
    {
        if (trackChanges)
        {
            return _context.Set<T>();
        }
        else
        {
            return _context.Set<T>().AsNoTracking();
        }
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        if (trackChanges)
        {
            return _context.Set<T>().Where(expression);
        }
        else
        {
            return _context.Set<T>().Where(expression).AsNoTracking();
        }
    }

    public void Create(T entity) => _context.Set<T>().Add(entity);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task CreateAndSaveAsync(T entity)
    {
        Create(entity);
        await SaveChangesAsync();
    }
    public async Task UpdateAndSaveAsync(T entity)
    {
        Update(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteAndSaveAsync(T entity)
    {
        Delete(entity);
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}