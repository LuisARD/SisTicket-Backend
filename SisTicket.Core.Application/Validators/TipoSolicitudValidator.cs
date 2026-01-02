using FluentValidation;
using SisTicket.Core.Application.DTOs.TipoSolicitud;

namespace SisTicket.Core.Application.Validators;

public class TipoSolicitudValidator : AbstractValidator<TipoSolicitudRequest>
{
    public TipoSolicitudValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.AreaId)
            .GreaterThan(0).WithMessage("Debe seleccionar un área válida");
    }
}
