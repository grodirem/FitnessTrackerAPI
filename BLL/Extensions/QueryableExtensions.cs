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
        string fieldName,
        bool descending)
    {
        return fieldName switch
        {
            "type" => descending
                ? query.OrderByDescending(w => w.Type)
                : query.OrderBy(w => w.Type),
            "duration" => descending
                ? query.OrderByDescending(w => w.Duration)
                : query.OrderBy(w => w.Duration),
            "calories" => descending
                ? query.OrderByDescending(w => w.Calories)
                : query.OrderBy(w => w.Calories),
            _ => descending
                ? query.OrderByDescending(w => w.Date)
                : query.OrderBy(w => w.Date)
        };
    }
}
