using AutoMapper;
using SisTicket.Core.Application.DTOs.TipoSolicitud;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class TipoSolicitudService : ITipoSolicitudService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TipoSolicitudService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TipoSolicitudResponse>> GetAllAsync()
    {
        var tipos = await _unitOfWork.TiposSolicitud.GetAllAsync();
        return _mapper.Map<IEnumerable<TipoSolicitudResponse>>(tipos);
    }

    public async Task<IEnumerable<TipoSolicitudResponse>> GetByAreaIdAsync(int areaId)
    {
        // Validar que existe el área
        var area = await _unitOfWork.Areas.GetByIdAsync(areaId);
        if (area == null)
            throw new NotFoundException(nameof(Area), areaId);

        var tipos = await _unitOfWork.TiposSolicitud.GetAllAsync();
        var tiposFiltrados = tipos.Where(t => t.AreaId == areaId);
        return _mapper.Map<IEnumerable<TipoSolicitudResponse>>(tiposFiltrados);
    }

    public async Task<TipoSolicitudResponse> GetByIdAsync(int id)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task<TipoSolicitudResponse> CreateAsync(TipoSolicitudRequest request)
    {
        // Validar que existe el área
        var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId);
        if (area == null)
            throw new NotFoundException(nameof(Area), request.AreaId);

        // Validar que no existe un tipo de solicitud con el mismo nombre en la misma área
        if (await _unitOfWork.TiposSolicitud.ExistsByNombreAndAreaAsync(request.Nombre, request.AreaId))
            throw new ValidationException($"Ya existe un tipo de solicitud con el nombre '{request.Nombre}' en el área '{area.Nombre}'");

        var tipo = _mapper.Map<TipoSolicitud>(request);
        await _unitOfWork.TiposSolicitud.AddAsync(tipo);
        await _unitOfWork.SaveChangesAsync();

        // Recargar con la relación Area
        tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(tipo.Id);
        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task<TipoSolicitudResponse> UpdateAsync(int id, TipoSolicitudRequest request)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        // Actualización parcial: solo actualizar campos con valores
        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            // Validar nombre único solo si cambia el nombre o el área
            if (tipo.Nombre != request.Nombre || tipo.AreaId != request.AreaId)
            {
                var targetAreaId = request.AreaId > 0 ? request.AreaId : tipo.AreaId;
                
                if (await _unitOfWork.TiposSolicitud.ExistsByNombreAndAreaExcludingIdAsync(request.Nombre, targetAreaId, id))
                {
                    var area = await _unitOfWork.Areas.GetByIdAsync(targetAreaId);
                    throw new ValidationException($"Ya existe un tipo de solicitud con el nombre '{request.Nombre}' en el área '{area?.Nombre}'");
                }
            }
            tipo.Nombre = request.Nombre;
        }

        // Solo actualizar descripción si se envía
        if (request.Descripcion != null)
        {
            tipo.Descripcion = request.Descripcion;
        }

        // Actualizar AreaId si se envía y es diferente
        if (request.AreaId > 0 && tipo.AreaId != request.AreaId)
        {
            // Validar que existe el área
            var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId);
            if (area == null)
                throw new NotFoundException(nameof(Area), request.AreaId);
            
            tipo.AreaId = request.AreaId;
        }

        await _unitOfWork.TiposSolicitud.UpdateAsync(tipo);
        await _unitOfWork.SaveChangesAsync();

        // Recargar con la relación Area
        tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(tipo.Id);
        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task DeleteAsync(int id)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        // Validar que no tenga solicitudes activas asociadas
        var tieneSolicitudesActivas = await _unitOfWork.Solicitudes.TieneTipoSolicitudSolicitudesActivasAsync(id);
        if (tieneSolicitudesActivas)
        {
            throw new ValidationException(
                $"No se puede eliminar el tipo de solicitud '{tipo.Nombre}' porque tiene solicitudes activas asociadas. " +
                "Solo se pueden eliminar tipos de solicitud sin solicitudes o con solicitudes en estado Rechazada o Cerrada."
            );
        }

        await _unitOfWork.TiposSolicitud.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
