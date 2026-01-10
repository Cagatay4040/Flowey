using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Step;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.StepValidators
{
    public class StepUpdateDTOValidator : AbstractValidator<StepUpdateDTO>
    {
        public StepUpdateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(50).WithMessage(Messages.MaxLengthExceeded);
        }
    }
}
