using System.Threading;

namespace TestUtility.Models
{
    public class ServiceResult
    {
        public int ThreadId { get; set; }

        public ApiResult ApiResult { get; set; }

        public ServiceResult(){}

        public ServiceResult(ApiResult apiResult)
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            ApiResult = apiResult;
        }
    }
}
