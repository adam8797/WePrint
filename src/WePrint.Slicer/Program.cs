using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Raven.Client.Documents;
using WePrint.Common;
using WePrint.Common.Models;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.ServiceDiscovery.Services;
using WePrint.Common.Slicer.Impl;
using WePrint.Common.Slicer.Interface;
using WePrint.Common.Slicer.Models;
using WePrint.Slicer.Impl;
using WePrint.Slicer.Interface;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WePrint.Slicer
{
    class Program
    {
        private static ManualResetEvent _exitEvent = new ManualResetEvent(false);

        private static IDocumentStore _store;
        private static ISlicer _slicer;
        private static string _instanceId;
        private static IConfiguration _config;

        static async Task Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Please provide config file");

            _instanceId = Guid.NewGuid().ToString("D");
            Console.WriteLine($"Instance ID: {_instanceId}");

            SetupConfiguration(args);

            await ConfigureRabbitMQ();
            await ConfigureRavenDB();
            ConfigureSlicer();

            Console.WriteLine("Setup Complete. Awaiting messages");
            _exitEvent.WaitOne();
        }

        static void SetupConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();
            foreach (var s in args)
            {
                builder.AddYamlFile(s);
            }
            _config = builder.Build();
        }

        static async Task ConfigureRavenDB()
        {
            Console.WriteLine("Configuring RavenDB...");
            var discovery = new DNSServiceDiscovery(_config);
            var config = await discovery.DiscoverAsync<RavenDBDiscoveredService>();
            _store = new DocumentStore()
            {
                Urls = config.Hosts.Select(x => $"http://{x}:8080").ToArray(),
                Database = config.DatabaseName,
            };
            _store.Initialize();
        }

        static async Task ConfigureRabbitMQ()
        {
            Console.WriteLine("Configuring RabbitMQ...");
            var discovery = new DNSServiceDiscovery(_config);
            var config = await discovery.DiscoverAsync<RabbitMQDiscoveredService>();

            var bus = RabbitHutch.CreateBus($"host={config.Hosts.First()};username={config.Username};password={config.Password}");
            bus.Receive<SliceJobMessage>(config.Queue, HandleMessage);
        }

        static void ConfigureSlicer()
        {
            Console.WriteLine("Configuring Slicer...");
            _slicer = new StubSlicer();
        }

        static void HandleMessage(SliceJobMessage message)
        {
            Console.WriteLine("Slice Job Message Received: " + message.SliceJobId);
            using (var session = _store.OpenSession())
            {
                var slicejob = session.Load<SlicerJob>(message.SliceJobId) ?? throw new Exception("Unable to load Slicer Job " + message.SliceJobId);
                Console.WriteLine("Loaded slicer job from database.");

                try
                {
                    var job = session.Load<JobModel>(slicejob.Job) ??
                              throw new Exception("Unable to load job " + slicejob.Job);
                    Console.WriteLine("Loaded Job from Database");

                    Console.WriteLine("Marking slice job as active");
                    slicejob.Status = SliceJobStatus.Processing;
                    slicejob.Worker = _instanceId;
                    session.SaveChanges();

                    if (slicejob.Files == null)
                        Console.WriteLine("Will slice ALL files");
                    else
                    {
                        Console.WriteLine("Slice List:");
                        foreach (var file in slicejob.Files)
                        {
                            Console.WriteLine(" - " + file);
                        }
                    }

                    Console.WriteLine("Starting Slice...");

                    var files = slicejob.Files ?? session.Advanced.Attachments.GetNames(job).Select(x => x.Name);

                    foreach (var file in files)
                    {
                        Console.WriteLine($"Slicing File {file}...");

                        Console.WriteLine("Retrieving file from database");
                        var attachment = session.Advanced.Attachments.Get(job, file);

                        Console.WriteLine("File Size: " + attachment.Details.Size);

                        var tmp = Path.GetTempFileName();
                        using (var tempFile = File.OpenWrite(tmp))
                        {
                            attachment.Stream.CopyTo(tempFile);
                        }

                        Console.WriteLine("File Downloaded");

                        Console.WriteLine("Starting Slicer...");

                        var watch = Stopwatch.StartNew();
                        var report = _slicer.Slice(file, tmp);
                        watch.Stop();

                        Console.WriteLine($"Slicing finished in {watch.Elapsed:g}");

                        job.SliceReports.Add(report);

                        if (_config["CleanupSliceJobs"].ToLower() == "true")
                            session.Delete(slicejob);
                        else
                            slicejob.Status = SliceJobStatus.Complete;

                        session.SaveChanges();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Caught: " + ex);
                    slicejob.Status = SliceJobStatus.Error;
                    session.SaveChanges();
                }

                Console.WriteLine();
            }
        }
    }
}
