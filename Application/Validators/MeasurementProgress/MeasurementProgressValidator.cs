using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.MeasurementProgress
{
    public class MeasurementProgressValidator : AbstractValidator<MeasurementProgressRequestDto>
    {
        public MeasurementProgressValidator()
        {
            RuleFor(x => x.IdAthlete)
                .NotNull().WithMessage("El campo id atleta no puede ser nulo")
                .NotEmpty().WithMessage("El campo id atleta no puede ser vacio");

            RuleFor(x => x.Gluteus)
                .NotNull().WithMessage("El campo glúteos no puede ser nulo")
                .NotEmpty().WithMessage("El campo glúteos no puede ser vacio");

            RuleFor(x => x.Biceps)
                .NotNull().WithMessage("El campo biceps no puede ser nulo")
                .NotEmpty().WithMessage("El campo biceps no puede ser vacio");

            RuleFor(x => x.Shoulders)
                .NotNull().WithMessage("El campo hombros no puede ser nulo")
                .NotEmpty().WithMessage("El campo hombros no puede ser vacio");

            RuleFor(x => x.Chest)
                .NotNull().WithMessage("El campo pecho no puede ser nulo")
                .NotEmpty().WithMessage("El campo pecho no puede ser vacio");

            RuleFor(x => x.Waist)
                .NotNull().WithMessage("El campo cintura no puede ser nulo")
                .NotEmpty().WithMessage("El campo cintura no puede ser vacio");

            RuleFor(x => x.Forearm)
                .NotNull().WithMessage("El campo antebrazos no puede ser nulo")
                .NotEmpty().WithMessage("El campo antebrazos no puede ser vacio");

            RuleFor(x => x.Thigh)
                .NotNull().WithMessage("El campo pierna no puede ser nulo")
                .NotEmpty().WithMessage("El campo pierna no puede ser vacio");

            RuleFor(x => x.Calf)
                .NotNull().WithMessage("El campo pantorrillas no puede ser nulo")
                .NotEmpty().WithMessage("El campo pantorrillas no puede ser vacio");

            RuleFor(x => x.Weight)
                .NotNull().WithMessage("El campo peso no puede ser nulo")
                .NotEmpty().WithMessage("El campo peso no puede ser vacio");

            RuleFor(x => x.Height)
                .NotNull().WithMessage("El campo altura no puede ser nulo")
                .NotEmpty().WithMessage("El campo altura no puede ser vacio");
        }
    }
}
