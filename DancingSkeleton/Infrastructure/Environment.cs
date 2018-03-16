using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure
{
    class Environment
    {
        readonly Type[] componentTypes;
        readonly Dictionary<Type, Func<Host, IEngine>> engines = new Dictionary<Type, Func<Host, IEngine>>();
        readonly List<Host> hosts = new List<Host>();
        readonly List<object> sharedServices = new List<object>();

        public Environment(params Type[] componentTypes)
        {
            this.componentTypes = componentTypes;
        }

        public void AddSharedService(object service)
        {
            sharedServices.Add(service);
        }

        public T GetSharedService<T>()
        {
            return sharedServices.Where(s => s is T).Cast<T>().FirstOrDefault();
        }

        public void AddEngine<T>(Func<Host, T> engineConstructor)
            where T : IEngine
        {
            engines[typeof(T)] = host => engineConstructor(host);
        }

        public void AddHost(string name, params Type[] engineTypes)
        {
            var host = new Host(name, this, componentTypes);
            foreach (var engineType in engineTypes)
            {
                host.AddEngine(engines[engineType](host));
            }
            hosts.Add(host);
        }

        public string[] GetHostsFor(Type componentType)
        {
            return hosts.Where(h => h.HostedComponents.Contains(componentType)).Select(h => h.Name).ToArray();
        }

        public async Task Start()
        {
            foreach (var host in hosts)
            {
                await host.Start();
            }
        }

        public async Task Stop()
        {
            foreach (var host in hosts)
            {
                await host.Stop();
            }
        }
    }
}