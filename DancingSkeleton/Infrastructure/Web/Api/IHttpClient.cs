using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure.Web.Api
{
    interface IHttpClient
    {
        Task<object> Send(object request, string prefix, string suffix);
    }
}