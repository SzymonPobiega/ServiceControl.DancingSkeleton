using System;

namespace DancingSkeleton.Infrastructure.Web.Api
{
    interface IEnvironment
    {
        string[] GetHostsFor(Type componentType);
        string HostName { get; }
        T GetSharedService<T>();
    }
}