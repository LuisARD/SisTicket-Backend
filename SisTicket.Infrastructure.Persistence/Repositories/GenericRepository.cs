using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Common;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null && entity is BaseEntity baseEntity)
        {
            // Soft delete: marcar como eliminado en lugar de eliminar físicamente
            baseEntity.Eliminado = true;
            baseEntity.Activo = false;
            baseEntity.FechaEliminacion = DateTime.UtcNow;
            baseEntity.FechaModificacion = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null;
    }
}
