using System;

namespace DancingSkeleton.Infrastructure
{
    interface IEnvironment
    {
        string[] GetHostsFor(Type componentType);
        string HostName { get; }
        T GetSharedService<T>();
    }
}