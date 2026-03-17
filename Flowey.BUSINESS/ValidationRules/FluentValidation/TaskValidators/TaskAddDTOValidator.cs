using Flowey.CORE.DTO.Task;
using Flowey.BUSINESS.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.SHARED.Constants;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.TaskValidators
{
    public class TaskAddDTOValidator : AbstractValidator<TaskAddDTO>
    {
        public TaskAddDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(100).WithMessage(Messages.MaxLengthExceeded);

            RuleFor(x => x.Description)
                .NotEmptyHtml().WithMessage(Messages.RequiredField)
                .MaximumLength(500).WithMessage(Messages.MaxLengthExceeded);

            When(x => x.Deadline.HasValue, () =>
            {
                RuleFor(x => x.Deadline)
                    .Must(d => d.Value >= DateTime.UtcNow)
                    .WithMessage(Messages.DeadlineCannotBeInThePast);
            });
        }
    }
}
