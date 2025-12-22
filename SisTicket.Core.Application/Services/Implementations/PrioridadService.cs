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

        _mapper.Map(request, prioridad);
        await _unitOfWork.Prioridades.UpdateAsync(prioridad);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PrioridadResponse>(prioridad);
    }

    public async Task DeleteAsync(int id)
    {
        var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(id);
        
        if (prioridad == null)
            throw new NotFoundException(nameof(Prioridad), id);

        await _unitOfWork.Prioridades.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
