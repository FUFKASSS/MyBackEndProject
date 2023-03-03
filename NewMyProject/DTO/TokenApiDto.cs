using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.Entities
{
    public class TokenApiDto : BaseValidationModel<TokenApiDto>
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public class LogInModelValidator : AbstractValidator<TokenApiDto>
    {
        public LogInModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.AccessToken).NotNull()
                                       .NotEmpty();

            RuleFor(x => x.RefreshToken).NotNull()
                                        .NotEmpty();
        }
    }
}
