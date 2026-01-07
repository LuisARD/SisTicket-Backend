using AutoMapper;
using SisTicket.Core.Application.DTOs.Usuario;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UsuarioResponse>> GetAllAsync()
    {
        var usuarios = await _unitOfWork.Usuarios.GetAllAsync();
        return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
    }

    public async Task<UsuarioResponse> GetByIdAsync(int id)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), id);

        return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse> CreateAsync(UsuarioRequest request, int usuarioActualId)
    {
        // VALIDAR PERMISO: Solo SuperAdmin puede crear usuarios
        await ValidarPermisoSuperAdmin(usuarioActualId, "crear usuarios");

        // Validar email único
        if (await _unitOfWork.Usuarios.ExistsByEmailAsync(request.Email))
            throw new ValidationException($"Ya existe un usuario con el email '{request.Email}'");

        // Validar nombre de usuario único
        if (await _unitOfWork.Usuarios.ExistsByNombreUsuarioAsync(request.NombreUsuario))
            throw new ValidationException($"Ya existe un usuario con el nombre de usuario '{request.NombreUsuario}'");

        // Validar que el área existe si se proporciona
        if (request.AreaId.HasValue)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId.Value);
            if (area == null)
                throw new NotFoundException(nameof(Area), request.AreaId.Value);
        }

        var usuario = new Usuario
        {
            NombreUsuario = request.NombreUsuario,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password), // ? BCrypt real
            Rol = (Rol)request.Rol,
            AreaId = request.AreaId
        };

        await _unitOfWork.Usuarios.AddAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task<UsuarioResponse> UpdateAsync(int id, UsuarioRequest request, int usuarioActualId)
    {
        // VALIDAR PERMISO: Solo SuperAdmin puede actualizar usuarios
        await ValidarPermisoSuperAdmin(usuarioActualId, "actualizar usuarios");

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), id);

        // Actualización parcial: solo actualizar campos con valores
        
        // Actualizar nombre de usuario si se envía
        if (!string.IsNullOrWhiteSpace(request.NombreUsuario))
        {
            var usuarioConNombre = await _unitOfWork.Usuarios.GetByNombreUsuarioAsync(request.NombreUsuario);
            if (usuarioConNombre != null && usuarioConNombre.Id != id)
                throw new ValidationException($"Ya existe un usuario con el nombre de usuario '{request.NombreUsuario}'");
            
            usuario.NombreUsuario = request.NombreUsuario;
        }

        // Actualizar nombre si se envía
        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            usuario.Nombre = request.Nombre;
        }

        // Actualizar apellido si se envía
        if (!string.IsNullOrWhiteSpace(request.Apellido))
        {
            usuario.Apellido = request.Apellido;
        }

        // Actualizar email si se envía
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var usuarioConEmail = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email);
            if (usuarioConEmail != null && usuarioConEmail.Id != id)
                throw new ValidationException($"Ya existe un usuario con el email '{request.Email}'");
            
            usuario.Email = request.Email;
        }

        // Actualizar rol si se envía (validar que sea un valor válido)
        if (request.Rol >= 0)
        {
            usuario.Rol = (Rol)request.Rol;
        }

        // Actualizar área si se envía
        if (request.AreaId.HasValue)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId.Value);
            if (area == null)
                throw new NotFoundException(nameof(Area), request.AreaId.Value);
            
            usuario.AreaId = request.AreaId;
        }

        // Solo actualizar password si se proporciona uno nuevo
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            usuario.PasswordHash = _passwordHasher.HashPassword(request.Password); // ? BCrypt real
        }

        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UsuarioResponse>(usuario);
    }

    public async Task DeleteAsync(int id, int usuarioActualId)
    {
        // VALIDAR PERMISO: Solo SuperAdmin puede eliminar usuarios
        await ValidarPermisoSuperAdmin(usuarioActualId, "eliminar usuarios");

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), id);

        // Validar que no se está intentando eliminar a sí mismo
        if (id == usuarioActualId)
            throw new ValidationException("No puede eliminar su propio usuario");

        // VALIDACIÓN: Solo se puede eliminar si está inactivo
        if (usuario.Activo)
        {
            throw new ValidationException(
                $"No se puede eliminar el usuario '{usuario.NombreUsuario}' porque está activo. " +
                "Primero debe desactivarlo usando el endpoint de cambio de estado."
            );
        }

        await _unitOfWork.Usuarios.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<UsuarioResponse>> GetByAreaIdAsync(int areaId)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(areaId);
        
        if (area == null)
            throw new NotFoundException(nameof(Area), areaId);

        var usuarios = await _unitOfWork.Usuarios.GetByAreaIdAsync(areaId);
        return _mapper.Map<IEnumerable<UsuarioResponse>>(usuarios);
    }

    public async Task<IEnumerable<UsuarioResponse>> GetGestoresByAreaIdAsync(int areaId)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(areaId);
        
        if (area == null)
            throw new NotFoundException(nameof(Area), areaId);

        var gestores = await _unitOfWork.Usuarios.GetGestoresByAreaIdAsync(areaId);
        return _mapper.Map<IEnumerable<UsuarioResponse>>(gestores);
    }

    public async Task<UsuarioResponse> CambiarEstadoAsync(int id, bool activo, int usuarioActualId)
    {
        // VALIDAR PERMISO: Solo SuperAdmin puede cambiar estado de usuarios
        await ValidarPermisoSuperAdmin(usuarioActualId, "cambiar estado de usuarios");

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), id);

        // Validar que no se está intentando desactivar a sí mismo
        if (id == usuarioActualId && !activo)
            throw new ValidationException("No puede desactivar su propio usuario");

        // Cambiar estado
        usuario.Activo = activo;

        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UsuarioResponse>(usuario);
    }

    // Método privado para validar que el usuario actual es SuperAdmin
    private async Task ValidarPermisoSuperAdmin(int usuarioActualId, string accion)
    {
        var usuarioActual = await _unitOfWork.Usuarios.GetByIdAsync(usuarioActualId);
        
        if (usuarioActual == null)
            throw new UnauthorizedException("Usuario no encontrado");

        if (!usuarioActual.EsSuperAdmin())
            throw new UnauthorizedException($"Solo el SuperAdmin puede {accion}");
    }
}
