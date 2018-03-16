using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DancingSkeleton.Infrastructure
{
    class Environment
    {
        readonly Type[] componentTypes;
        readonly Dictionary<Type, Func<Particle, IEngine>> engines = new Dictionary<Type, Func<Particle, IEngine>>();
        readonly List<Particle> particles = new List<Particle>();
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

        public void AddEngine<T>(Func<Particle, T> engineConstructor)
            where T : IEngine
        {
            engines[typeof(T)] = host => engineConstructor(host);
        }

        public void AddParticle(string name, params Type[] engineTypes)
        {
            var host = new Particle(name, this, componentTypes);
            foreach (var engineType in engineTypes)
            {
                host.AddEngine(engines[engineType](host));
            }
            particles.Add(host);
        }

        public string[] GetHostsFor(Type componentType)
        {
            return particles.Where(h => h.HostedComponents.Contains(componentType)).Select(h => h.Name).ToArray();
        }

        public async Task Start()
        {
            foreach (var host in particles)
            {
                await host.Start();
            }
        }

        public async Task Stop()
        {
            foreach (var host in particles)
            {
                await host.Stop();
            }
        }
    }
}