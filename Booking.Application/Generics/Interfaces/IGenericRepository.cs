


using System.Linq.Expressions;


namespace Booking.Application.Generics.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync( T entity, CancellationToken ct= default);
        void Remove(T entity);
        Task<T> GetByIdAsync(Guid id, CancellationToken ct= default);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }
}

