using SisTicket.Core.Application.DTOs.Auth;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Buscar usuario por nombre de usuario
        var usuario = await _unitOfWork.Usuarios.GetByNombreUsuarioAsync(request.NombreUsuario);

        if (usuario == null)
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // NOTA: La validación de password se hará en el controller con IPasswordHasher
        // El token JWT también se generará en el controller con IJwtService
        // Este servicio solo retorna la información del usuario

        var response = new LoginResponse
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Area = usuario.Area?.Nombre,
            Token = string.Empty // Se asignará en el controller
        };

        return response;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        // La validación JWT se manejará por el middleware de autenticación
        return await Task.FromResult(!string.IsNullOrEmpty(token));
    }
}
