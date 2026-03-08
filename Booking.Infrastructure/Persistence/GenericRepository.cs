using Booking.Application.Generics.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Booking.Infrastructure.Persistence;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly BookingDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    public async Task<T> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync([id], ct);
        return entity!; 
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _dbSet.AnyAsync(predicate, ct);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _dbContext.SaveChangesAsync(ct);

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    => await _dbSet.FirstOrDefaultAsync(predicate, ct);

   public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
    => await _dbSet.Where(predicate).ToListAsync(ct);
}


