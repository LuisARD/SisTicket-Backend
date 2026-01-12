using FluentValidation;
using SisTicket.Core.Application.DTOs.Usuario;

namespace SisTicket.Core.Application.Validators;

public class CambiarPasswordRequestValidator : AbstractValidator<CambiarPasswordRequest>
{
    public CambiarPasswordRequestValidator()
    {
        RuleFor(x => x.PasswordActual)
            .NotEmpty().WithMessage("La contraseña actual es requerida");

        RuleFor(x => x.PasswordNueva)
            .NotEmpty().WithMessage("La nueva contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches(@"[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .Matches(@"[-*@!#$%^&+=]").WithMessage("La contraseña debe contener al menos un símbolo (-, *, @, !, #, $, %, etc.)")
            .NotEqual(x => x.PasswordActual).WithMessage("La nueva contraseña debe ser diferente a la actual");

        RuleFor(x => x.ConfirmarPassword)
            .NotEmpty().WithMessage("Debe confirmar la contraseña")
            .Equal(x => x.PasswordNueva).WithMessage("Las contraseñas no coinciden");
    }
}
