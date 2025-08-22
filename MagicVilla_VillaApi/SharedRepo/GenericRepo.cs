using AutoMapper;
using MagicVilla_VillaApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaApi.SharedRepo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private readonly ApplicationDbContext _Context;
        public GenericRepo( ApplicationDbContext Context)
        {
            _dbSet = Context.Set<T>();
            _Context = Context;

        }

        public async Task AddAsync(T entity)
        {
          await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter != null)
            {
                return await _dbSet.AsNoTracking().Where(filter).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public  IQueryable<T> GetAllAsyncAsQuery()
        {
            return  _dbSet.AsQueryable();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracking = false)
        {
            if (filter == null)
            {
                return null;
            }

            if (tracking)
            {
                return await _dbSet.FirstOrDefaultAsync(filter);
            }
            else
            {
                return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
            }
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
           await _Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
           _dbSet.Update(entity);
            await SaveAsync();
        }
    }
}
