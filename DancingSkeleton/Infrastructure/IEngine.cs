using System;
using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure
{
    interface IEngine
    {
        Task Start();
        Task Stop();
        void Add(object component);
        bool CanHandle(Type componentType);
    }
}