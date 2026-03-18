using Flowey.CORE.DTO.User;
using Flowey.SHARED.Constants;
using FluentValidation;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.UserValidators
{
    public class UserLoginDTOValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.EmailRequired)
                .EmailAddress().WithMessage(Messages.InvalidEmailFormat);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded);
        }
    }
}
