using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DTO.Comment;
using Flowey.SHARED.Constants;
using FluentValidation;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.CommentValidators
{
    public class CommentUpdateDTOValidator : AbstractValidator<CommentUpdateDTO>
    {
        public CommentUpdateDTOValidator()
        {
            RuleFor(x => x.Content)
                .NotEmptyHtml().WithMessage(Messages.RequiredField)
                .MaximumLength(1000).WithMessage(Messages.MaxLengthExceeded)
                .MinimumLength(5).WithMessage(Messages.MinLengthError);
        }
    }
}
