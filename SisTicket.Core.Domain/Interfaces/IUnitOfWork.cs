namespace SisTicket.Core.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository Usuarios { get; }
    IAreaRepository Areas { get; }
    ITipoSolicitudRepository TiposSolicitud { get; }
    IPrioridadRepository Prioridades { get; }
    ISolicitudRepository Solicitudes { get; }
    IComentarioRepository Comentarios { get; }
    IAdjuntoRepository Adjuntos { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
