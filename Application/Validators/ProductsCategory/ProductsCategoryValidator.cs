using Application.Dtos.Request;
using FluentValidation;

namespace Application.Validators.ProductsCategory
{
    public class ProductsCategoryValidator : AbstractValidator<CategoryProductsRequestDto>
    {
        public ProductsCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotNull().WithMessage("El campo nombre no puede ser nulo")
                .NotEmpty().WithMessage("El campo nombre no puede ser vacio");
        }
    }
}
