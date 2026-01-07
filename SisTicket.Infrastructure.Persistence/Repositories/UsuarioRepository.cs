using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
    }

    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email && u.Activo);
    }

    public async Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _dbSet.AnyAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
    }

    public async Task<IEnumerable<Usuario>> GetByAreaIdAsync(int areaId)
    {
        return await _dbSet
            .Include(u => u.Area)
            .Where(u => u.AreaId == areaId && u.Activo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetGestoresByAreaIdAsync(int areaId)
    {
        return await _dbSet
            .Include(u => u.Area)
            .Where(u => u.AreaId == areaId && u.Rol == Rol.Gestor && u.Activo)
            .ToListAsync();
    }

    public override async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Area)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public override async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _dbSet
            .Include(u => u.Area)
            .OrderBy(u => u.Activo ? 0 : 1)  // Primero activos, luego inactivos
            .ThenBy(u => u.NombreUsuario)
            .ToListAsync();
    }
}
