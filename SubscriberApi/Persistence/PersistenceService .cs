namespace SubscriberApi.Persistence;

using Microsoft.EntityFrameworkCore;
using SubscriberApi.Data;

 

public class PersistenceService : IPersistenceService
{
    private readonly ApplicationDbContext _context;

    public PersistenceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : class
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync<T>(CancellationToken cancellationToken = default)
        where T : class
    {
        return await _context.Set<T>()
         .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public void Update<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<T>().Update(entity);
    }

    public void Remove<T>(T entity) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        _context.Set<T>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}