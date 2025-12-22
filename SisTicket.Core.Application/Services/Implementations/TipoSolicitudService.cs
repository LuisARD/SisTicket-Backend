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

    public async Task<TipoSolicitudResponse> GetByIdAsync(int id)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task<TipoSolicitudResponse> CreateAsync(TipoSolicitudRequest request)
    {
        if (await _unitOfWork.TiposSolicitud.ExistsByNombreAsync(request.Nombre))
            throw new ValidationException($"Ya existe un tipo de solicitud con el nombre '{request.Nombre}'");

        var tipo = _mapper.Map<TipoSolicitud>(request);
        await _unitOfWork.TiposSolicitud.AddAsync(tipo);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task<TipoSolicitudResponse> UpdateAsync(int id, TipoSolicitudRequest request)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        _mapper.Map(request, tipo);
        await _unitOfWork.TiposSolicitud.UpdateAsync(tipo);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TipoSolicitudResponse>(tipo);
    }

    public async Task DeleteAsync(int id)
    {
        var tipo = await _unitOfWork.TiposSolicitud.GetByIdAsync(id);
        
        if (tipo == null)
            throw new NotFoundException(nameof(TipoSolicitud), id);

        await _unitOfWork.TiposSolicitud.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
