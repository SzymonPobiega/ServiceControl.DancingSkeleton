﻿using System;
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

            var cluster = new Cluster(
                typeof(MonitoringComponent), 
                typeof(DebuggingComponent), 
                typeof(DebuggingScatterGatherComponent),
                typeof(RecoverabilityImportComponent),
                typeof(RecoverabilityApiComponent),
                typeof(UptimeMonitorComponent));

            //Configure the engines
            cluster.RegisterEngine(p => new WebEngine(webInfra, p.Name, p));
            cluster.RegisterEngine(p => new MetricsEngine(queueInfra));
            cluster.RegisterEngine(p => new AuditsEngine(queueInfra));
            cluster.RegisterEngine(p => new ErrorsEngine(queueInfra, p));
            cluster.RegisterEngine(p => new HeartbeatsEngine(queueInfra));

            //Add a failed message DB as a shared service. It mimics a central remote persistent database.
            cluster.AddSharedService(new FailedMessageDatabase());

            //First particle has metrics and HB engines enabled in addition to the web engine. It allows it to run the Endpoint Lifecycle and Performance Monitoring components
            cluster.AddParticle("1", typeof(WebEngine), typeof(MetricsEngine), typeof(HeartbeatsEngine));

            //Second and third particle have audit engines. They connect to the same queue in competing consumers manner. 
            cluster.AddParticle("2", typeof(WebEngine), typeof(AuditsEngine));
            cluster.AddParticle("3", typeof(WebEngine), typeof(AuditsEngine));

            //Last particle runs only the errors engine which stores failed messages. Because the Recoverability has a central database the web API for
            //the Recoverability does not have to be hosted in the same particle.
            cluster.AddParticle("4", typeof(ErrorsEngine));

            await cluster.Start();

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
                            (GetMonitoringDataResponse) await webInfra.Send(new GetMonitoringData(), "monitoring");
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
                            (GetMessagesResponse)await webInfra.Send(new GetMessages(), "messages");
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
                            (GetFailedMessagesResponse)await webInfra.Send(new GetFailedMessages(), "errors");
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
                            (GetLastHeartbeatResponse)await webInfra.Send(new GetLastHeartbeat(), "heartbeats");
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
