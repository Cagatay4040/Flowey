using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Comment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.CommentValidators
{
    public class CommentAddDTOValidator : AbstractValidator<CommentAddDTO>
    {
        public CommentAddDTOValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(1000).WithMessage(Messages.MaxLengthExceeded)
                .MinimumLength(5).WithMessage(Messages.MinLengthError);
        }
    }
}
