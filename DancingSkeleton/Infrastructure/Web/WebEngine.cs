using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DancingSkeleton.Infrastructure.Web.Api;

namespace DancingSkeleton.Infrastructure.Web
{
    class WebEngine : IEngine, IHttpClient
    {
        readonly WebInfrastructure infrastructure;
        readonly string prefix;
        readonly IEnvironment environment;
        readonly List<IController> controllers = new List<IController>();
        CancellationTokenSource stopTokenSource;
        Task[] receiveTasks;

        public WebEngine(WebInfrastructure infrastructure, string prefix, IEnvironment environment)
        {
            this.infrastructure = infrastructure;
            this.prefix = prefix;
            this.environment = environment;
        }

        public Task Start()
        {
            stopTokenSource = new CancellationTokenSource();

            receiveTasks = controllers.Select(c =>
            {
                var receiver = infrastructure.Listen(prefix, c.UrlSuffix, stopTokenSource.Token);
                return Task.Run(() => ProcessMessages(c, receiver));
            }).ToArray();

            return Task.CompletedTask;
        }

        void ProcessMessages(IController controller, IEnumerable<HttpEnvelope> receiver)
        {
            foreach (var envelope in receiver)
            {
                if (controller != null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            var response = await controller.Process(envelope.Request, this, environment);
                            envelope.Reply(response);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            envelope.ReplyError();
                        }
                        
                    });
                }
                else
                {
                    envelope.ReplyError();
                }
            }
        }

        public Task Stop()
        {
            stopTokenSource.Cancel();
            stopTokenSource.Dispose();
            return Task.WhenAll(receiveTasks);
        }

        public void Add(object component)
        {
            controllers.Add((IController)component);
        }

        public bool CanHandle(Type componentType)
        {
            return componentType == typeof(IController);
        }

        public async Task<object> Send(object request, string prefix, string suffix)
        {
            var result = await infrastructure.Send(request, prefix, suffix);
            return result;
        }
    }
}