using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Infrastructure
{
    class Host : IEnvironment
    {
        readonly List<IEngine> engines = new List<IEngine>();
        readonly Environment environment;
        readonly Type[] componentTypes;
        public string Name { get; }
        public List<Type> HostedComponents { get; } = new List<Type>();

        public Host(string name, Environment environment, params Type[] componentTypes)
        {
            this.environment = environment;
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
            return environment.GetHostsFor(componentType);
        }

        public string HostName => Name;
        public T GetSharedService<T>()
        {
            return environment.GetSharedService<T>();
        }
    }
}