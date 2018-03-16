using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DancingSkeleton.Capabilities.Debugging;
using DancingSkeleton.Capabilities.EndpointLifecycle;
using DancingSkeleton.Capabilities.PerformanceMonitoring;
using DancingSkeleton.Capabilities.Recoverability;
using DancingSkeleton.Infrastructure;
using DancingSkeleton.Infrastructure.Queue;
using DancingSkeleton.Infrastructure.Web;
using DancingSkeleton.Ops.Audits;
using DancingSkeleton.Ops.Audits.Api;
using DancingSkeleton.Ops.Errors;
using DancingSkeleton.Ops.Errors.Api;
using DancingSkeleton.Ops.Heartbeats;
using DancingSkeleton.Ops.Heartbeats.Api;
using DancingSkeleton.Ops.Metrics;
using DancingSkeleton.Ops.Metrics.Api;
using Environment = DancingSkeleton.Infrastructure.Environment;

namespace DancingSkeleton
{
    class Program
    {
        static void Main(string[] args)
        {
            Start().GetAwaiter().GetResult();
        }

        static async Task Start()
        {
            var r = new Random();
            string[] endpoints = {"A" , "B", "C", "D", "E", "F"};

            var webInfra = new WebInfrastructure();
            var queueInfra = new QueueInfrastructure();

            var environment = new Environment(
                typeof(MonitoringComponent), 
                typeof(DebuggingComponent), 
                typeof(DebuggingScatterGatherComponent),
                typeof(RecoverabilityImportComponent),
                typeof(RecoverabilityApiComponent),
                typeof(UptimeMonitorComponent));

            environment.AddEngine(host => new WebEngine(webInfra, host.Name, host));
            environment.AddEngine(host => new MetricsEngine(queueInfra));
            environment.AddEngine(host => new AuditsEngine(queueInfra));
            environment.AddEngine(host => new ErrorsEngine(queueInfra, host));
            environment.AddEngine(host => new HeartbeatsEngine(queueInfra));

            environment.AddSharedService(new FailedMessageDatabase());

            environment.AddHost("1", typeof(WebEngine), typeof(MetricsEngine), typeof(HeartbeatsEngine));
            environment.AddHost("2", typeof(WebEngine), typeof(AuditsEngine));
            environment.AddHost("3", typeof(WebEngine), typeof(AuditsEngine));
            environment.AddHost("4", typeof(ErrorsEngine));

            await environment.Start();

            Console.WriteLine("Press a to send a metrics message");
            while (true)
            {
                var key = Console.ReadKey().KeyChar;
                if (key == 'M')
                {
                    var values = Enumerable.Range(0, 3).Select(i => r.Next(100)).Select(i => (decimal)i).ToArray();
                    queueInfra.SendTo(new Message(new MetricsMessage(DateTime.UtcNow, values)), "metrics");
                }
                else if (key == 'm')
                {
                    try
                    {
                        var response =
                            (GetMonitoringDataResponse) await webInfra.Send(new GetMonitoringData(), "1", "monitoring");
                        Console.WriteLine("Response: " + string.Join(",", response.Measurements));
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine("Cannot handle request");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                }
                else if (key == 'A')
                {
                    var sender = endpoints[r.Next(endpoints.Length)];
                    var receiver = endpoints[r.Next(endpoints.Length)];
                    queueInfra.SendTo(new Message(new ProcessedMessage(sender, receiver)), "audit");
                }
                else if (key == 'a')
                {
                    try
                    {
                        var response =
                            (GetMessagesResponse)await webInfra.Send(new GetMessages(), "1", "messages");
                        Console.WriteLine("Response: " + string.Join(",", response.Messages.Select(m => m.SendingEndpoint + m.ProcessingEndpoint)));
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine("Cannot handle request");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                else if (key == 'E')
                {
                    var sender = endpoints[r.Next(endpoints.Length)];
                    var receiver = endpoints[r.Next(endpoints.Length)];
                    queueInfra.SendTo(new Message(new FailedMessage(sender, receiver)), "error");
                }
                else if (key == 'e')
                {
                    try
                    {
                        var response =
                            (GetFailedMessagesResponse)await webInfra.Send(new GetFailedMessages(), "1", "errors");
                        Console.WriteLine("Response: " + string.Join(",", response.Messages.Select(m => m.SendingEndpoint + m.ProcessingEndpoint)));
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine("Cannot handle request");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                else if (key == 'H')
                {
                    queueInfra.SendTo(new Message(new HeartbeatMessage(DateTime.UtcNow)), "heartbeat");
                }
                else if (key == 'h')
                {
                    try
                    {
                        var response =
                            (GetLastHeartbeatResponse)await webInfra.Send(new GetLastHeartbeat(), "1", "heartbeats");
                        Console.WriteLine("Response: " + response.LastHeartbeat);
                    }
                    catch (TaskCanceledException e)
                    {
                        Console.WriteLine("Cannot handle request");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
        }
    }
}
