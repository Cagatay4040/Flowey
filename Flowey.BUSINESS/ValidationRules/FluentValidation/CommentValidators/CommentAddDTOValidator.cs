using Flowey.CORE.DTO.Comment;
using Flowey.BUSINESS.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.SHARED.Constants;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.CommentValidators
{
    public class CommentAddDTOValidator : AbstractValidator<CommentAddDTO>
    {
        public CommentAddDTOValidator()
        {
            RuleFor(x => x.Content)
                .NotEmptyHtml().WithMessage(Messages.RequiredField)
                .MaximumLength(1000).WithMessage(Messages.MaxLengthExceeded)
                .MinimumLength(5).WithMessage(Messages.MinLengthError);
        }
    }
}
