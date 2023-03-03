using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class UpdateUserAndProfileDto : BaseValidationModel<UpdateUserAndProfileDto>
    {
        public long PhoneNumber { get; set; }
        public string UserName {get;set;}
        public string? Password {get;set;}
        public string Email {get;set;}
    }
    public class UpdateUserAndProfileModelValidator : AbstractValidator<UpdateUserAndProfileDto>
    {
        public UpdateUserAndProfileModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.UserName).NotNull()
                                    .NotEmpty()
                                    .MinimumLength(4);

            RuleFor(x => x.Password).MinimumLength(6);

            RuleFor(x => x.Email).NotNull()
                                 .NotEmpty()
                                 .EmailAddress();
        }
    }
}
