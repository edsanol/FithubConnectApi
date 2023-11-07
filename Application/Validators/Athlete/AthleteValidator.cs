using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.Athlete
{
    public class AthleteValidator : AbstractValidator<AthleteRequestDto>
    {
        public AthleteValidator()
        {
            RuleFor(x => x.AthleteName)
                .NotNull().WithMessage("El campo nombre no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre no puede ser vacio");

            RuleFor(x => x.AthleteLastName)
                .NotNull().WithMessage("El campo apellido no puede ser nulo")
                .NotEmpty().WithMessage("El campo apellido no puede ser vacio");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("El campo email no puede ser nulo")
                .NotEmpty().WithMessage("El campo email no puede ser vacio")
                .EmailAddress().WithMessage("El campo email no es valido");

            RuleFor(x => x.PhoneNumber)
                .NotNull().WithMessage("El campo número de teléfono no puede ser nulo")
                .NotEmpty().WithMessage("El campo número de teléfono no puede ser vacio");

            RuleFor(x => x.BirthDate)
                .NotNull().WithMessage("El campo fecha de nacimiento no puede ser nulo")
                .NotEmpty().WithMessage("El campo fecha de nacimiento no puede ser vacio");

            RuleFor(x => x.Genre)
                .NotNull().WithMessage("El campo género no puede ser nulo")
                .NotEmpty().WithMessage("El campo género no puede ser vacio");

            RuleFor(x => x.IdGym)
                .NotNull().WithMessage("El campo id gimnasio no puede ser nulo")
                .NotEmpty().WithMessage("El campo id gimnasio no puede ser vacio");

            RuleFor(x => x.GymName)
                .NotNull().WithMessage("El campo nombre gimnasio no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre gimnasio no puede ser vacio");

            RuleFor(x => x.RegisterDate)
                .NotNull().WithMessage("El campo fecha de registro no puede ser nulo")
                .NotEmpty().WithMessage("El campo fecha de registro no puede ser vacio");
        }
    }
}
