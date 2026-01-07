using AutoMapper;
using SisTicket.Core.Application.DTOs.Prioridad;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class PrioridadService : IPrioridadService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PrioridadService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PrioridadResponse>> GetAllAsync()
    {
        var prioridades = await _unitOfWork.Prioridades.GetOrderedByNivelAsync();
        return _mapper.Map<IEnumerable<PrioridadResponse>>(prioridades);
    }

    public async Task<PrioridadResponse> GetByIdAsync(int id)
    {
        var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(id);
        
        if (prioridad == null)
            throw new NotFoundException(nameof(Prioridad), id);

        return _mapper.Map<PrioridadResponse>(prioridad);
    }

    public async Task<PrioridadResponse> CreateAsync(PrioridadRequest request)
    {
        if (await _unitOfWork.Prioridades.ExistsByNombreAsync(request.Nombre))
            throw new ValidationException($"Ya existe una prioridad con el nombre '{request.Nombre}'");

        var prioridad = _mapper.Map<Prioridad>(request);
        await _unitOfWork.Prioridades.AddAsync(prioridad);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PrioridadResponse>(prioridad);
    }

    public async Task<PrioridadResponse> UpdateAsync(int id, PrioridadRequest request)
    {
        var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(id);
        
        if (prioridad == null)
            throw new NotFoundException(nameof(Prioridad), id);

        // Actualización parcial: solo actualizar campos con valores
        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            // Validar nombre único solo si cambió
            if (prioridad.Nombre != request.Nombre && 
                await _unitOfWork.Prioridades.ExistsByNombreAsync(request.Nombre))
            {
                throw new ValidationException($"Ya existe una prioridad con el nombre '{request.Nombre}'");
            }
            prioridad.Nombre = request.Nombre;
        }

        // Solo actualizar nivel si se envía un valor mayor a 0
        if (request.Nivel > 0)
        {
            prioridad.Nivel = request.Nivel;
        }

        // Solo actualizar descripción si se envía
        if (request.Descripcion != null)
        {
            prioridad.Descripcion = request.Descripcion;
        }

        await _unitOfWork.Prioridades.UpdateAsync(prioridad);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PrioridadResponse>(prioridad);
    }

    public async Task DeleteAsync(int id)
    {
        var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(id);
        
        if (prioridad == null)
            throw new NotFoundException(nameof(Prioridad), id);

        // Validar que no tenga solicitudes activas asociadas
        var tieneSolicitudesActivas = await _unitOfWork.Solicitudes.TienePrioridadSolicitudesActivasAsync(id);
        if (tieneSolicitudesActivas)
        {
            throw new ValidationException(
                $"No se puede eliminar la prioridad '{prioridad.Nombre}' porque tiene solicitudes activas asociadas. " +
                "Solo se pueden eliminar prioridades sin solicitudes o con solicitudes en estado Rechazada o Cerrada."
            );
        }

        await _unitOfWork.Prioridades.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
