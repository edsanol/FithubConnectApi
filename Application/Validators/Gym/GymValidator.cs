using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.Gym
{
    public class GymValidator : AbstractValidator<GymRequestDto>
    {
        public GymValidator()
        {
            RuleFor(x => x.GymName)
                .NotNull().WithMessage("El campo nombre no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre no puede ser vacio");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("El campo email no puede ser nulo")
                .NotEmpty().WithMessage("El campo email no puede ser vacio")
                .EmailAddress().WithMessage("El campo email no es valido");

            //RuleFor(x => x.Password)
            //    .NotNull().WithMessage("El campo contraseña no puede ser nulo")
            //    .NotEmpty().WithMessage("El campo contraseña no puede ser vacio");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("El campo dirección no puede ser nulo")
                .NotEmpty().WithMessage("El campo dirección no puede ser vacio");

            RuleFor(x => x.PhoneNumber)
                .NotNull().WithMessage("El campo número de teléfono no puede ser nulo")
                .NotEmpty().WithMessage("El campo número de teléfono no puede ser vacio");

            RuleFor(x => x.SubscriptionPlan)
                .NotNull().WithMessage("El campo plan de suscripción no puede ser nulo")
                .NotEmpty().WithMessage("El campo plan de suscripción no puede ser vacio");

            RuleFor(x => x.Comments)
                .NotNull().WithMessage("El campo comentarios no puede ser nulo")
                .NotEmpty().WithMessage("El campo comentarios no puede ser vacio");

            RuleFor(x => x.Nit)
                .NotNull().WithMessage("El campo nit no puede ser nulo")
                .NotEmpty().WithMessage("El campo nit no puede ser vacio");

            RuleFor(x => x.AccessTypeIds)
                .NotEmpty().WithMessage("Debe seleccionar al menos un tipo de acceso.")
                .Must(ids => ids.Distinct().Count() == ids.Count())
                .WithMessage("No se permiten tipos de acceso duplicados.");
        }
    }
}
