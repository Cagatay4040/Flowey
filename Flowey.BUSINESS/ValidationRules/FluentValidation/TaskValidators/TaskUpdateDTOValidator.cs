using Flowey.BUSINESS.Extensions;
using Flowey.CORE.DTO.Task;
using Flowey.SHARED.Constants;
using FluentValidation;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.TaskValidators
{
    public class TaskUpdateDTOValidator : AbstractValidator<TaskUpdateDTO>
    {
        public TaskUpdateDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(100).WithMessage(Messages.MaxLengthExceeded);

            RuleFor(x => x.Description)
                .NotEmptyHtml().WithMessage(Messages.RequiredField)
                .MaximumLength(500).WithMessage(Messages.MaxLengthExceeded);
        }
    }
}
