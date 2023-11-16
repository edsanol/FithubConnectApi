using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.Discount
{
    public class DiscountValidator : AbstractValidator<DiscountRequestDto>
    {
        public DiscountValidator()
        {
            RuleFor(x => x.DiscountPercentage)
                .NotNull().WithMessage("El campo porcentaje de descuento no puede ser nulo")
                .NotEmpty().WithMessage("El campo porcentaje de descuento no puede ser vacio");

            RuleFor(x => x.StartDate)
                .NotNull().WithMessage("El campo fecha de inicio no puede ser nulo")
                .NotEmpty().WithMessage("El campo fecha de inicio no puede ser vacio");

            RuleFor(x => x.EndDate)
                .NotNull().WithMessage("El campo fecha de fin no puede ser nulo")
                .NotEmpty().WithMessage("El campo fecha de fin no puede ser vacio");

            RuleFor(x => x.IdMembership)
                .NotNull().WithMessage("El campo id membresia no puede ser nulo")
                .NotEmpty().WithMessage("El campo id membresia no puede ser vacio");

            RuleFor(x => x.IdGym)
                .NotNull().WithMessage("El campo id gimnasio no puede ser nulo")
                .NotEmpty().WithMessage("El campo id gimnasio no puede ser vacio");
        }
    }
}
