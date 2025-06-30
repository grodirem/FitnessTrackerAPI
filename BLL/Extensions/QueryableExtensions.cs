using Common.Enums;
using DAL.Entities;
using System.Linq.Expressions;

namespace BLL.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<Workout> OrderByField(
    this IQueryable<Workout> query,
    SortType? sortBy,
    bool sortDescending = false)
    {
        return sortBy switch
        {
            SortType.Type => sortDescending
                ? query.OrderByDescending(w => w.Type)
                : query.OrderBy(w => w.Type),

            SortType.Duration => sortDescending
                ? query.OrderByDescending(w => w.Duration)
                : query.OrderBy(w => w.Duration),

            SortType.Calories => sortDescending
                ? query.OrderByDescending(w => w.Calories)
                : query.OrderBy(w => w.Calories),

            _ => sortDescending 
                ? query.OrderByDescending(w => w.Date)
                : query.OrderBy(w => w.Date)
        };
    }
}
