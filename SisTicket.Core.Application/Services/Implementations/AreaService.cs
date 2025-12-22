using AutoMapper;
using SisTicket.Core.Application.DTOs.Area;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class AreaService : IAreaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AreaResponse>> GetAllAsync()
    {
        var areas = await _unitOfWork.Areas.GetAllAsync();
        return _mapper.Map<IEnumerable<AreaResponse>>(areas);
    }

    public async Task<AreaResponse> GetByIdAsync(int id)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(id);
        
        if (area == null)
            throw new NotFoundException(nameof(Area), id);

        return _mapper.Map<AreaResponse>(area);
    }

    public async Task<AreaResponse> CreateAsync(AreaRequest request)
    {
        if (await _unitOfWork.Areas.ExistsByNombreAsync(request.Nombre))
            throw new ValidationException($"Ya existe un área con el nombre '{request.Nombre}'");

        var area = _mapper.Map<Area>(request);
        await _unitOfWork.Areas.AddAsync(area);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AreaResponse>(area);
    }

    public async Task<AreaResponse> UpdateAsync(int id, AreaRequest request)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(id);
        
        if (area == null)
            throw new NotFoundException(nameof(Area), id);

        var existeNombre = await _unitOfWork.Areas.ExistsByNombreAsync(request.Nombre);
        var areaConMismoNombre = await _unitOfWork.Areas.GetByNombreAsync(request.Nombre);
        
        if (existeNombre && areaConMismoNombre?.Id != id)
            throw new ValidationException($"Ya existe un área con el nombre '{request.Nombre}'");

        _mapper.Map(request, area);
        await _unitOfWork.Areas.UpdateAsync(area);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AreaResponse>(area);
    }

    public async Task DeleteAsync(int id)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(id);
        
        if (area == null)
            throw new NotFoundException(nameof(Area), id);

        await _unitOfWork.Areas.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
