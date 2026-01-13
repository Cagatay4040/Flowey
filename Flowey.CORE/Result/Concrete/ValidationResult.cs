using Flowey.CORE.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowey.CORE.Result.Concrete
{
    public class ValidationResult : IResult
    {
        public ResultStatus ResultStatus { get; } = ResultStatus.Error;
        public string Message { get; }
        public Exception Exception { get; } = null;

        public List<ValidationErrorDetail> ValidationErrors { get; }

        public ValidationResult(string message, List<ValidationErrorDetail> errors)
        {
            Message = message;
            ValidationErrors = errors;
        }
    }
}
