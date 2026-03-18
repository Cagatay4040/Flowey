using Flowey.CORE.Result.Concrete;

namespace Flowey.CORE.Result.Abstract
{
    public interface IResult
    {
        public ResultStatus ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }
    }
}
