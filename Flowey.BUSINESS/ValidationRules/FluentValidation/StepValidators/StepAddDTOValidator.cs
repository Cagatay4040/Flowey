using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DTO.Step;
using Flowey.SHARED.Constants;
using FluentValidation;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.StepValidators
{
    public class StepAddDTOValidator : AbstractValidator<StepAddDTO>
    {
        public StepAddDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded)
                .NotContainHtml().WithMessage(Messages.HtmlTagsNotAllowed);
        }
    }
}
