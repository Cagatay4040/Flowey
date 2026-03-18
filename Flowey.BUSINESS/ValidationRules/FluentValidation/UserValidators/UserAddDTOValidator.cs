using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DTO.User;
using Flowey.SHARED.Constants;
using FluentValidation;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.UserValidators
{
    public class UserAddDTOValidator : AbstractValidator<UserAddDTO>
    {
        public UserAddDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Messages.EmailRequired)
                .EmailAddress().WithMessage(Messages.InvalidEmailFormat);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded)
                .NotContainHtml().WithMessage(Messages.HtmlTagsNotAllowed);

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded)
                .NotContainHtml().WithMessage(Messages.HtmlTagsNotAllowed);
        }
    }
}
