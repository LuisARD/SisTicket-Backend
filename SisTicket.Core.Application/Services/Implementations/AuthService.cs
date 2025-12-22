using AutoMapper;
using SisTicket.Core.Application.DTOs.Auth;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // Buscar usuario por nombre de usuario
        var usuario = await _unitOfWork.Usuarios.GetByNombreUsuarioAsync(request.NombreUsuario);

        if (usuario == null)
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Validar password
        // NOTA: Esta es una validación temporal
        // En la siguiente fase se implementará con BCrypt
        if (!ValidarPassword(request.Password, usuario.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Generar token JWT
        // NOTA: Implementación temporal
        // En la siguiente fase se implementará JWT real
        var token = GenerarTokenTemporal(usuario.Id, usuario.Rol.ToString());

        var response = new LoginResponse
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Area = usuario.Area?.Nombre,
            Token = token
        };

        return response;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        // NOTA: Implementación temporal
        // En la siguiente fase se implementará validación JWT real
        return await Task.FromResult(!string.IsNullOrEmpty(token));
    }

    // NOTA: Método temporal de validación de password
    // En la siguiente fase se reemplazará con BCrypt
    private bool ValidarPassword(string password, string passwordHash)
    {
        // Implementación temporal - DEBE SER REEMPLAZADA
        return passwordHash == $"TEMP_HASH_{password}";
    }

    // NOTA: Método temporal de generación de token
    // En la siguiente fase se reemplazará con JWT real
    private string GenerarTokenTemporal(int usuarioId, string rol)
    {
        return $"TEMP_TOKEN_{usuarioId}_{rol}_{Guid.NewGuid()}";
    }
}
