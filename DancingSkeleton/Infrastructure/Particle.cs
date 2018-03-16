using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Infrastructure
{
    class Particle : IEnvironment
    {
        readonly List<IEngine> engines = new List<IEngine>();
        readonly Cluster cluster;
        readonly Type[] componentTypes;
        public string Name { get; }
        public List<Type> HostedComponents { get; } = new List<Type>();

        public Particle(string name, Cluster cluster, params Type[] componentTypes)
        {
            this.cluster = cluster;
            this.componentTypes = componentTypes;
            Name = name;
        }

        public void AddEngine(IEngine engine)
        {
            engines.Add(engine);
        }

        public async Task Start()
        {
            Console.WriteLine($"Starting host {Name}...");

            foreach (var type in componentTypes)
            {
                var interfaces = type.GetInterfaces();
                var handlingEngines = interfaces.Select(GetEngine).Where(e => e != null).ToArray();

                if (interfaces.Length != handlingEngines.Length)
                {
                    Console.WriteLine($"Skipping component {type.Name}.");
                    //Some engines are missing, do not run this component
                    continue;
                }
                var instance = Activator.CreateInstance(type);
                foreach (var engine in handlingEngines)
                {
                    Console.WriteLine($"Attaching component {type.Name} to {engine.GetType().Name}.");
                    engine.Add(instance);
                }
                HostedComponents.Add(type);
            }

            foreach (var engine in engines)
            {
                await engine.Start();
            }
            Console.WriteLine("Started");
        }

        IEngine GetEngine(Type type)
        {
            return engines.FirstOrDefault(e => e.CanHandle(type));
        }

        public async Task Stop()
        {
            foreach (var engine in engines)
            {
                await engine.Stop();
            }
        }

        public string[] GetHostsFor(Type componentType)
        {
            return cluster.GetHostsFor(componentType);
        }

        public string HostName => Name;
        public T GetSharedService<T>()
        {
            return cluster.GetSharedService<T>();
        }
    }
}