using Flowey.BUSINESS.Constants;
using Flowey.BUSINESS.DTO.Attachment;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.BUSINESS.ValidationRules.FluentValidation.AttachmentValidators
{
    public class FileUploadDTOValidator : AbstractValidator<FileUploadDTO>
    {
        public FileUploadDTOValidator()
        {
            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Messages.FileRequired)
                .Must(x => x.Length > 0).WithMessage(Messages.FileRequired)
                .Must(HaveValidSize).WithMessage(string.Format(Messages.FileTooLarge, FileSettings.MaxFileSizeMB))
                .Must(HaveValidExtension).WithMessage(string.Format(Messages.InvalidFileType, string.Join(", ", FileSettings.AllowedExtensions)));
        }

        private bool HaveValidSize(IFormFile file)
        {
            if (file == null) return false;
            return file.Length <= FileSettings.MaxFileSystemBytes;
        }

        private bool HaveValidExtension(IFormFile file)
        {
            if (file == null) return false;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            return FileSettings.AllowedExtensions.Contains(ext);
        }
    }
}
