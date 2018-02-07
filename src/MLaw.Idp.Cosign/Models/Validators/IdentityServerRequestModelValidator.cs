using FluentValidation;

namespace MLaw.Idp.Cosign.Models.Validators
{
    public class IdentityServerRequestModelValidator : AbstractValidator<IdentityServerRequestModel>
    {
        public IdentityServerRequestModelValidator()
        {
            RuleFor(x => x.ClientId).NotNull().NotEmpty().Length(3, 100);
            RuleFor(x => x.ClientSecret).NotNull().NotEmpty().MinimumLength(5);
            RuleFor(x => x.Code).NotNull().NotEmpty().MinimumLength(10);
        }
    }
}