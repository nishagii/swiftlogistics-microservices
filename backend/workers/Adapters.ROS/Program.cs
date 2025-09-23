// /backend/workers/Adapters.ROS/Program.cs
using Adapters.ROS.Consumers;
using MassTransit;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient("RosClient", client =>
        {
            var baseAddress = hostContext.Configuration["Services:RosApiUrl"];
            client.BaseAddress = new Uri(baseAddress!);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
        
        // configure MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderReceivedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(hostContext.Configuration["RabbitMQ:Host"], "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();