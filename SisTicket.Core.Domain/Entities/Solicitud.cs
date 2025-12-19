using SisTicket.Core.Domain.Common;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Domain.Entities;

public class Solicitud : BaseEntity
{
    public string NumeroSolicitud { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Nueva;
    
    public int SolicitanteId { get; set; }
    public int? GestorAsignadoId { get; set; }
    public int TipoSolicitudId { get; set; }
    public int PrioridadId { get; set; }
    public int AreaId { get; set; }
    
    public Usuario Solicitante { get; set; } = null!;
    public Usuario? GestorAsignado { get; set; }
    public TipoSolicitud TipoSolicitud { get; set; } = null!;
    public Prioridad Prioridad { get; set; } = null!;
    public Area Area { get; set; } = null!;
    
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public bool PuedeSerEditadaPorSolicitante()
    {
        return Estado == EstadoSolicitud.Nueva && GestorAsignadoId == null;
    }

    public bool TieneGestorAsignado()
    {
        return GestorAsignadoId.HasValue;
    }

    public bool EstaEnEstado(EstadoSolicitud estado)
    {
        return Estado == estado;
    }

    public void AsignarGestor(int gestorId)
    {
        if (gestorId <= 0)
            throw new ArgumentException("El ID del gestor debe ser válido", nameof(gestorId));
        
        // NOTA: La validación de permisos (solo Admin/SuperAdmin pueden asignar)
        // se debe realizar en la capa de Application antes de llamar este método
        
        GestorAsignadoId = gestorId;
        
        if (Estado == EstadoSolicitud.Nueva)
        {
            Estado = EstadoSolicitud.EnProceso;
        }
    }

    public void CambiarEstado(EstadoSolicitud nuevoEstado)
    {
        if (!EsTransicionValida(nuevoEstado))
            throw new InvalidOperationException($"No se puede cambiar de {Estado} a {nuevoEstado}");
        
        Estado = nuevoEstado;
    }

    private bool EsTransicionValida(EstadoSolicitud nuevoEstado)
    {
        return (Estado, nuevoEstado) switch
        {
            (EstadoSolicitud.Nueva, EstadoSolicitud.EnProceso) => true,
            (EstadoSolicitud.Nueva, EstadoSolicitud.Rechazada) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Resuelta) => true,
            (EstadoSolicitud.EnProceso, EstadoSolicitud.Rechazada) => true,
            (EstadoSolicitud.Resuelta, EstadoSolicitud.Cerrada) => true,
            (EstadoSolicitud.Resuelta, EstadoSolicitud.EnProceso) => true,
            _ => false
        };
    }
}
