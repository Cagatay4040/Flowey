using Flowey.CORE.Constants;
using Flowey.BUSINESS.DTO.Task;
using Flowey.BUSINESS.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
