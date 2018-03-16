using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure.Web
{
    class HttpEnvelope
    {
        readonly TaskCompletionSource<object> responseSource;

        public HttpEnvelope(object request, string urlSuffix, TaskCompletionSource<object> responseSource)
        {
            this.responseSource = responseSource;
            UrlSuffix = urlSuffix;
            Request = request;
        }

        public object Request { get; }
        public string UrlSuffix { get; }

        public void Reply(object response)
        {
            responseSource.SetResult(response);
        }

        public void ReplyError()
        {
            responseSource.SetCanceled();
        }
    }
}