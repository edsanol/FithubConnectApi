using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.Membership
{
    public class MembershipVaidator : AbstractValidator<MembershipRequestDto>
    {
        public MembershipVaidator()
        {
            RuleFor(x => x.MembershipName)
                .NotNull().WithMessage("El campo nombre no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre no puede ser vacio");

            RuleFor(x => x.Cost)
                .NotNull().WithMessage("El campo precio no puede ser nulo")
                .NotEmpty().WithMessage("El campo precio no puede ser vacio");

            RuleFor(x => x.DurationInDays)
                .NotNull().WithMessage("El campo duración en días no puede ser nulo")
                .NotEmpty().WithMessage("El campo duración en días no puede ser vacio");

            RuleFor(x => x.IdGym)
                .NotNull().WithMessage("El campo id gimnasio no puede ser nulo")
                .NotEmpty().WithMessage("El campo id gimnasio no puede ser vacio");
        }
    }
}
