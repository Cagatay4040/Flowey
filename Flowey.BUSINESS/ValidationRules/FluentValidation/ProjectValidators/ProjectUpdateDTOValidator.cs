using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Project;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.ProjectValidators
{
    public class ProjectUpdateDTOValidator : AbstractValidator<ProjectUpdateDTO>
    {
        public ProjectUpdateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(100).WithMessage(Messages.MaxLengthExceeded);

            RuleFor(x => x.ProjectKey)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(30).WithMessage(Messages.MaxLengthExceeded);
        }
    }
}
