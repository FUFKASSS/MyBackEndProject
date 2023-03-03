using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class CartDtoForAdmin : BaseValidationModel<CartDtoForAdmin>
    {
        public int id { get; set; }
        public int status { get; set; }
    }

    public class CartForAdminModelValidator : AbstractValidator<CartDtoForAdmin>
    {
        public CartForAdminModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.id).NotNull()
                                    .NotEmpty();

            RuleFor(x => x.status).NotNull()
                                    .NotEmpty();
        }
    }
}
