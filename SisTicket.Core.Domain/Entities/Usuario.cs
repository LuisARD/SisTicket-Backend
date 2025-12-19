using SisTicket.Core.Domain.Common;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Rol Rol { get; set; }
    public int? AreaId { get; set; }
    
    public Area? Area { get; set; }
    public ICollection<Solicitud> SolicitudesCreadas { get; set; } = new List<Solicitud>();
    public ICollection<Solicitud> SolicitudesAsignadas { get; set; } = new List<Solicitud>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

    public string ObtenerNombreCompleto() => $"{Nombre} {Apellido}";
    
    public bool EsSolicitante() => Rol == Rol.Solicitante;
    public bool EsGestor() => Rol == Rol.Gestor;
    public bool EsAdmin() => Rol == Rol.Admin;
    public bool EsSuperAdmin() => Rol == Rol.SuperAdmin;
    public bool TienePermisoAdministrativo() => Rol == Rol.Admin || Rol == Rol.SuperAdmin;
}
