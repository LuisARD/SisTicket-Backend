using Microsoft.EntityFrameworkCore.Storage;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;
using SisTicket.Infrastructure.Persistence.Repositories;

namespace SisTicket.Infrastructure.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        
        Usuarios = new UsuarioRepository(_context);
        Areas = new AreaRepository(_context);
        TiposSolicitud = new TipoSolicitudRepository(_context);
        Prioridades = new PrioridadRepository(_context);
        Solicitudes = new SolicitudRepository(_context);
        Comentarios = new ComentarioRepository(_context);
        Adjuntos = new AdjuntoRepository(_context);
    }

    public IUsuarioRepository Usuarios { get; }
    public IAreaRepository Areas { get; }
    public ITipoSolicitudRepository TiposSolicitud { get; }
    public IPrioridadRepository Prioridades { get; }
    public ISolicitudRepository Solicitudes { get; }
    public IComentarioRepository Comentarios { get; }
    public IAdjuntoRepository Adjuntos { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
