using Flowey.CORE.Result.Abstract;
using Flowey.SHARED.Enums;

namespace Flowey.CORE.Result.Concrete
{
    public class DataResult<T> : IDataResult<T>
    {
        public ResultStatus ResultStatus { get; }
        public string Message { get; }
        public Exception Exception { get; }
        public T Data { get; }

        public DataResult(ResultStatus resultStatus, T data)
        {
            ResultStatus = resultStatus;
            Data = data;
        }
        public DataResult(ResultStatus resultStatus, T data, string message)
        {
            ResultStatus = resultStatus;
            Data = data;
            Message = message;
        }
        public DataResult(ResultStatus resultStatus, T data, string message, Exception exception)
        {
            ResultStatus = resultStatus;
            Data = data;
            Message = message;
            Exception = exception;
        }
    }
}
