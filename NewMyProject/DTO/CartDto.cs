using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class CartDto : BaseValidationModel<CartDto>
    {
        public int productId { get; set; }
        public int weightId { get; set; }
        public int typeId { get; set; }
    }

    public class CartModelValidator : AbstractValidator<CartDto>
    {
        public CartModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.productId).NotNull()
                                    .NotEmpty();

            RuleFor(x => x.weightId).NotNull()
                                    .NotEmpty();
            RuleFor(x => x.typeId).NotNull()
                                    .NotEmpty();
        }
    }
}
