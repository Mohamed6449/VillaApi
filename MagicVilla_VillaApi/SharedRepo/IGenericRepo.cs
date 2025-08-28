using System.Linq.Expressions;

namespace MagicVilla_VillaApi.SharedRepo
{
    public interface IGenericRepo<T> where T : class
    {
        public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        public Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracking=false);

        public Task<T> AddAsync (T entity);

        public Task UpdateAsync(T entity);

        public Task RemoveAsync(T entity);

        public IQueryable<T> GetAllAsyncAsQuery();
        public Task SaveAsync();

    }
}
