using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.UserValidators
{
    public class UserPasswordChangeDTOValidator : AbstractValidator<UserPasswordChangeDTO>
    {
        public UserPasswordChangeDTOValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage(Messages.RequiredField);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MinimumLength(6).WithMessage(Messages.PasswordMinLength) 
                .NotEqual(x => x.OldPassword).WithMessage(Messages.NewPasswordCannotBeSame);

            RuleFor(x => x.NewPasswordConfirm)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .Equal(x => x.NewPassword).WithMessage(Messages.PasswordsDoNotMatch);
        }
    }
}
