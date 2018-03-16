using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure.Web.Api
{
    interface IController
    {
        string UrlSuffix { get; }
        Task<object> Process(object request, IHttpClient client, IEnvironment environment);
    }
}