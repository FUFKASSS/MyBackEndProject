using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class RegisterDto : BaseValidationModel<RegisterDto>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
    }

    public class RegInModelValidator : AbstractValidator<RegisterDto>
    {
        public RegInModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserName).NotNull()
                                    .NotEmpty()
                                    .MinimumLength(4);

            RuleFor(x => x.Password).NotNull()
                                    .NotEmpty()
                                    .MinimumLength(6);

            RuleFor(x => x.Email).NotNull()
                                 .NotEmpty()
                                 .EmailAddress();

            RuleFor(x => x.PhoneNumber).NotNull()
                                       .NotEmpty();
        }
    }
}
