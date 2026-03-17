using Flowey.CORE.DTO.Project;
using Flowey.BUSINESS.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.SHARED.Constants;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.ProjectValidators
{
    public class ProjectAddDTOValidator : AbstractValidator<ProjectAddDTO>
    {
        public ProjectAddDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(100).WithMessage(Messages.MaxLengthExceeded)
                .NotContainHtml().WithMessage(Messages.HtmlTagsNotAllowed);

            RuleFor(x => x.ProjectKey)
                .NotEmpty().WithMessage(Messages.RequiredField)
                .MaximumLength(30).WithMessage(Messages.MaxLengthExceeded)
                .NotContainHtml().WithMessage(Messages.HtmlTagsNotAllowed);
        }
    }
}
