using DibariBot.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DibariBot.Database.Extensions;

public static class DbSetExtensions
{
    public static async Task UpsertAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> findQuery, Action<T> upsertFunc) where T : DbModel, new()
    {
        var exists = await dbSet.FirstOrDefaultAsync(predicate: findQuery);

        if (exists != null)
        {
            upsertFunc(exists);
        }
        else
        {
            var instance = new T();

            upsertFunc(instance);

            dbSet.Add(instance);
        }
    }

    public static async Task<T> GetOrAddAsync<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> findQuery, Func<T> addFunc) where T : DbModel, new()
    {
        var exists = await dbSet.FirstOrDefaultAsync(predicate: findQuery);

        if (exists != null)
        {
            return exists;
        }
        else
        {
            var toAdd = addFunc();

            dbSet.Add(toAdd);

            return toAdd;
        }
    }
}