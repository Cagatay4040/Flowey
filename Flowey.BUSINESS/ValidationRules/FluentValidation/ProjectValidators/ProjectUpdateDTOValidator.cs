using Flowey.CORE.DTO.Project;
using Flowey.SHARED.Constants;
using FluentValidation;

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
