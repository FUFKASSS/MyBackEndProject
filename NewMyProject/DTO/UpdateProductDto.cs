using FluentValidation;
using NewMyProject.Common;

namespace NewMyProject.DTO
{
    public class UpdateProductDto : BaseValidationModel<UpdateProductDto>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Category { get; set; }
        public int Rating { get; set; }
        public decimal Price { get; set; }
    }
    public class UpdateProductModelValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductModelValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Category).NotNull();
            RuleFor(x => x.Rating).NotNull();
            RuleFor(x => x.Price).NotNull();
        }
    }
}
