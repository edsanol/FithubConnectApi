using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.Products
{
    public class ProductEditValidator : AbstractValidator<EditProductRequestDto>
    {
        public ProductEditValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("El campo nombre no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre no puede ser vacio");

            RuleFor(x => x.Description)
                .NotNull().WithMessage("El campo descripción no puede ser nulo")
                .NotEmpty().WithMessage("El campo descripción no puede ser vacio");

            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage("El campo id de categoría no puede ser nulo");

            RuleFor(x => x.BasePrice)
                .NotNull().WithMessage("El campo precio base no puede ser nulo");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("El campo precio no puede ser nulo");
        }
    }
}
