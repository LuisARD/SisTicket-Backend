using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Auth;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Interfaces;
using WebApp.SisTicket.Services;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Autenticación de Usuarios con JWT y Cookies HTTP-Only
/// </summary>
[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthController(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Login de usuario - Genera JWT y lo almacena en cookie HTTP-Only
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Buscar usuario
        var usuario = await _unitOfWork.Usuarios.GetByNombreUsuarioAsync(request.NombreUsuario);

        if (usuario == null)
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Verificar password con BCrypt
        if (!_passwordHasher.VerifyPassword(request.Password, usuario.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Detectar si tiene la contraseña temporal por defecto
        bool tienePasswordTemporal = _passwordHasher.VerifyPassword("Password@88", usuario.PasswordHash);

        // Generar token JWT
        var token = _jwtService.GenerateToken(usuario.Id, usuario.NombreUsuario, usuario.Rol.ToString());

        // Almacenar token en cookie HTTP-Only (más seguro que localStorage)
        Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,  // No accesible desde JavaScript (previene XSS)
            Secure = true,    // Solo HTTPS en producción
            SameSite = SameSiteMode.Strict,  // Protección CSRF
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        // Respuesta con el campo tienePasswordTemporal
        var response = new LoginResponse
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Area = usuario.Area?.Nombre,
            Token = token,
            TienePasswordTemporal = tienePasswordTemporal
        };

        return Ok(response);
    }

    /// <summary>
    /// Logout - Elimina la cookie de autenticación
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");
        return Ok(new { message = "Sesión cerrada correctamente" });
    }

    /// <summary>
    /// Obtiene información del usuario actual desde el token JWT
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var usuarioId = GetCurrentUserId();
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);

        if (usuario == null)
        {
            throw new UnauthorizedException();
        }

        // Detectar si tiene la contraseña temporal por defecto
        bool tienePasswordTemporal = _passwordHasher.VerifyPassword("Password@88", usuario.PasswordHash);

        var response = new LoginResponse
        {
            Id = usuario.Id,
            NombreUsuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Area = usuario.Area?.Nombre,
            Token = "Autenticado",
            TienePasswordTemporal = tienePasswordTemporal
        };

        return Ok(response);
    }
}
