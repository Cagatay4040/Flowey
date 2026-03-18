using Flowey.CORE.Result.Abstract;

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
