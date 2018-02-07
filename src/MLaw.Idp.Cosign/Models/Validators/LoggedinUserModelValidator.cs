using FluentValidation;

namespace MLaw.Idp.Cosign.Models.Validators
{
    public class LoggedinUserModelValidator : AbstractValidator<LoggedinUserModel>
    {
        public LoggedinUserModelValidator()
        {
            RuleFor(x => x.UserName).NotNull().NotEmpty().Length(3, 100);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Name).NotNull().NotEmpty().Length(1, 100);
        }
    }
}
