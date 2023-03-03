using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class LoginDto : BaseValidationModel<LoginDto>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; } 
        public long? PhoneNumber { get; set; }
    }

    public class LogInModelValidator : AbstractValidator<LoginDto>
    {
        public LogInModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.UserName).NotNull()
                                    .NotEmpty()
                                    .MinimumLength(4);

            RuleFor(x => x.Password).NotNull()
                                    .NotEmpty()
                                    .MinimumLength(6);
        }
    }
}
