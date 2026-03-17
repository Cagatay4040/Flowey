using Flowey.CORE.DTO.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.SHARED.Constants;

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
