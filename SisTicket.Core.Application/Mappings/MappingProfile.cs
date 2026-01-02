using AutoMapper;
using SisTicket.Core.Application.DTOs.Adjunto;
using SisTicket.Core.Application.DTOs.Area;
using SisTicket.Core.Application.DTOs.Comentario;
using SisTicket.Core.Application.DTOs.Prioridad;
using SisTicket.Core.Application.DTOs.Solicitud;
using SisTicket.Core.Application.DTOs.TipoSolicitud;
using SisTicket.Core.Application.DTOs.Usuario;
using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeos de Usuario
        CreateMap<Usuario, UsuarioResponse>()
            .ForMember(dest => dest.NombreCompleto, 
                opt => opt.MapFrom(src => src.ObtenerNombreCompleto()))
            .ForMember(dest => dest.Rol, 
                opt => opt.MapFrom(src => src.Rol.ToString()))
            .ForMember(dest => dest.AreaNombre, 
                opt => opt.MapFrom(src => src.Area != null ? src.Area.Nombre : null));

        CreateMap<UsuarioRequest, Usuario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Area, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitudesCreadas, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitudesAsignadas, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore());

        // Mapeos de Solicitud
        CreateMap<Solicitud, SolicitudResponse>()
            .ForMember(dest => dest.Estado, 
                opt => opt.MapFrom(src => src.Estado.ToString()))
            .ForMember(dest => dest.SolicitanteNombre, 
                opt => opt.MapFrom(src => src.Solicitante.ObtenerNombreCompleto()))
            .ForMember(dest => dest.GestorAsignadoNombre, 
                opt => opt.MapFrom(src => src.GestorAsignado != null ? src.GestorAsignado.ObtenerNombreCompleto() : null))
            .ForMember(dest => dest.TipoSolicitudNombre, 
                opt => opt.MapFrom(src => src.TipoSolicitud.Nombre))
            .ForMember(dest => dest.PrioridadNombre, 
                opt => opt.MapFrom(src => src.Prioridad.Nombre))
            .ForMember(dest => dest.AreaNombre, 
                opt => opt.MapFrom(src => src.Area.Nombre));

        CreateMap<SolicitudRequest, Solicitud>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.NumeroSolicitud, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.Ignore())
            .ForMember(dest => dest.SolicitanteId, opt => opt.Ignore())
            .ForMember(dest => dest.GestorAsignadoId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Solicitante, opt => opt.Ignore())
            .ForMember(dest => dest.GestorAsignado, opt => opt.Ignore())
            .ForMember(dest => dest.TipoSolicitud, opt => opt.Ignore())
            .ForMember(dest => dest.Prioridad, opt => opt.Ignore())
            .ForMember(dest => dest.Area, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore());

        // Mapeos de Comentario
        CreateMap<Comentario, ComentarioResponse>()
            .ForMember(dest => dest.UsuarioNombre, 
                opt => opt.MapFrom(src => src.Usuario.ObtenerNombreCompleto()));

        CreateMap<ComentarioRequest, Comentario>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Solicitud, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore());

        // Mapeos de Area
        CreateMap<Area, AreaResponse>();
        
        CreateMap<AreaRequest, Area>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
            .ForMember(dest => dest.Solicitudes, opt => opt.Ignore());

        // Mapeos de Prioridad
        CreateMap<Prioridad, PrioridadResponse>();
        
        CreateMap<PrioridadRequest, Prioridad>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Solicitudes, opt => opt.Ignore());

        // Mapeos de TipoSolicitud
        CreateMap<TipoSolicitud, TipoSolicitudResponse>()
            .ForMember(dest => dest.AreaNombre, 
                opt => opt.MapFrom(src => src.Area.Nombre));
        
        CreateMap<TipoSolicitudRequest, TipoSolicitud>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
            .ForMember(dest => dest.Activo, opt => opt.Ignore())
            .ForMember(dest => dest.Area, opt => opt.Ignore())
            .ForMember(dest => dest.Solicitudes, opt => opt.Ignore());

        // Mapeos de Adjunto
        CreateMap<Adjunto, AdjuntoResponse>()
            .ForMember(dest => dest.TamanoLegible, 
                opt => opt.MapFrom(src => FormatearTamano(src.TamanoBytes)))
            .ForMember(dest => dest.CargadoPorNombre, 
                opt => opt.MapFrom(src => src.CargadoPor.ObtenerNombreCompleto()));
    }

    private static string FormatearTamano(long bytes)
    {
        string[] sufijos = { "B", "KB", "MB", "GB" };
        int contador = 0;
        decimal numero = bytes;
        
        while (Math.Round(numero / 1024) >= 1)
        {
            numero /= 1024;
            contador++;
        }
        
        return $"{numero:n1} {sufijos[contador]}";
    }
}
