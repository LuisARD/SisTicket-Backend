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
    private readonly IAuditoriaService _auditoriaService;

    public UsuarioService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IPasswordHasher passwordHasher,
        IAuditoriaService auditoriaService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _auditoriaService = auditoriaService;
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
            PasswordHash = _passwordHasher.HashPassword("Password@88"),
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

        // Capturar valores ANTES del cambio
        var valoresAntiguos = new
        {
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            AreaId = usuario.AreaId,
            Area = usuario.Area?.Nombre
        };

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
            usuario.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }

        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        // Recargar entidad con relaciones para obtener el nombre del área actualizada
        usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);

        // Capturar valores DESPUÉS del cambio
        var valoresNuevos = new
        {
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            AreaId = usuario.AreaId,
            Area = usuario.Area?.Nombre
        };

        // Obtener información del usuario actual
        var usuarioActual = await _unitOfWork.Usuarios.GetByIdAsync(usuarioActualId);

        // Registrar auditoría con valores antes y después
        await _auditoriaService.RegistrarAsync(
            usuarioActualId,
            usuarioActual.NombreUsuario,
            usuarioActual.Rol.ToString(),
            TipoAccion.Actualizar,
            "Usuarios",
            id,
            $"PUT /api/usuarios/{id}",
            "Usuario modificado exitosamente",
            valoresAntiguos: valoresAntiguos,
            valoresNuevos: valoresNuevos,
            ipAddress: null,
            exitoso: true
        );

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

        // Soft delete: el método DeleteAsync del repositorio marca Eliminado = true
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

        // Capturar estado ANTES del cambio
        var valoresAntiguos = new
        {
            Activo = usuario.Activo
        };

        // Cambiar estado
        usuario.Activo = activo;

        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        // Capturar estado DESPUÉS del cambio
        var valoresNuevos = new
        {
            Activo = usuario.Activo
        };

        // Obtener información del usuario actual
        var usuarioActual = await _unitOfWork.Usuarios.GetByIdAsync(usuarioActualId);

        // Registrar auditoría
        await _auditoriaService.RegistrarAsync(
            usuarioActualId,
            usuarioActual.NombreUsuario,
            usuarioActual.Rol.ToString(),
            activo ? TipoAccion.ActivarUsuario : TipoAccion.DesactivarUsuario,
            "Usuarios",
            id,
            $"PATCH /api/usuarios/{id}/estado",
            activo ? "Usuario activado exitosamente" : "Usuario desactivado exitosamente",
            valoresAntiguos: valoresAntiguos,
            valoresNuevos: valoresNuevos,
            ipAddress: null,
            exitoso: true
        );

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

    public async Task CambiarMiPasswordAsync(int usuarioId, CambiarPasswordRequest request)
    {
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), usuarioId);

        // VALIDACIÓN 1: Verificar password actual
        if (!_passwordHasher.VerifyPassword(request.PasswordActual, usuario.PasswordHash))
            throw new ValidationException("La contraseña actual es incorrecta");

        // VALIDACIÓN 2: Verificar que no sea la misma password
        if (_passwordHasher.VerifyPassword(request.PasswordNueva, usuario.PasswordHash))
            throw new ValidationException("La nueva contraseña debe ser diferente a la actual");

        // VALIDACIÓN 3: Si tiene password temporal, NO puede reutilizarla
        bool tienePasswordTemporal = _passwordHasher.VerifyPassword("Password@88", usuario.PasswordHash);
        
        if (tienePasswordTemporal && request.PasswordNueva == "Password@88")
            throw new ValidationException("No puede usar la contraseña temporal como nueva contraseña");

        // Actualizar contraseña
        usuario.PasswordHash = _passwordHasher.HashPassword(request.PasswordNueva);
        
        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();

        // Registrar en auditoría
        var usuarioActual = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        await _auditoriaService.RegistrarAsync(
            usuarioId,
            usuarioActual.NombreUsuario,
            usuarioActual.Rol.ToString(),
            TipoAccion.Actualizar,
            "Usuarios",
            usuarioId,
            "POST /api/usuarios/cambiar-mi-password",
            "Contraseña cambiada exitosamente",
            valoresAntiguos: new { CambioPassword = "Anterior" },
            valoresNuevos: new { CambioPassword = DateTime.UtcNow },
            ipAddress: null,
            exitoso: true
        );
    }

    public async Task RestablecerPasswordAsync(int usuarioId, int usuarioActualId)
    {
        // VALIDACIÓN 1: Solo SuperAdmin puede restablecer contraseñas
        await ValidarPermisoSuperAdmin(usuarioActualId, "restablecer contraseñas");
        
        // VALIDACIÓN 2: Verificar que el usuario existe
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), usuarioId);
        
        // VALIDACIÓN 3: No puede restablecer su propia contraseña
        if (usuarioId == usuarioActualId)
            throw new ValidationException("No puede restablecer su propia contraseña");
        
        // Capturar valores antes del cambio
        var valoresAntiguos = new
        {
            PasswordRestablecida = false,
            Usuario = usuario.NombreUsuario
        };
        
        // Restablecer a password temporal por defecto
        usuario.PasswordHash = _passwordHasher.HashPassword("Password@88");
        
        await _unitOfWork.Usuarios.UpdateAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
        
        // Capturar valores después del cambio
        var valoresNuevos = new
        {
            PasswordRestablecida = true,
            PasswordTemporal = "Password@88"
        };
        
        // Registrar en auditoría
        var usuarioActual = await _unitOfWork.Usuarios.GetByIdAsync(usuarioActualId);
        await _auditoriaService.RegistrarAsync(
            usuarioActualId,
            usuarioActual.NombreUsuario,
            usuarioActual.Rol.ToString(),
            TipoAccion.Actualizar,
            "Usuarios",
            usuarioId,
            $"POST /api/usuarios/{usuarioId}/restablecer-password",
            $"Contraseña del usuario '{usuario.NombreUsuario}' restablecida a password temporal por SuperAdmin",
            valoresAntiguos: valoresAntiguos,
            valoresNuevos: valoresNuevos,
            ipAddress: null,
            exitoso: true
        );
    }
}
