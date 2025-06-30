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

    public async Task<List<T>> FindAllAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        return trackChanges
            ? await _context.Set<T>().ToListAsync(cancellationToken)
            : await _context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        return trackChanges
            ? await _context.Set<T>().Where(expression).ToListAsync(cancellationToken)
            : await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> FindFirstByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        var query = trackChanges
            ? _context.Set<T>().Where(expression)
            : _context.Set<T>().Where(expression).AsNoTracking();

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public void Create(T entity) => _context.Set<T>().Add(entity);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}