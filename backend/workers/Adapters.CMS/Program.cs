using Adapters.CMS.Consumers;
using MassTransit;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // --- Dependency Injection Configuration ---
        
        // 1. Configure HttpClientFactory to communicate with the mock CMS
        // We configure it once here, so we don't have to worry about managing HttpClient lifetimes.
        services.AddHttpClient("CmsClient", client =>
        {
            var baseAddress = hostContext.Configuration["Services:CmsApiUrl"];
            client.BaseAddress = new Uri(baseAddress!);
            
            // This handler allows HttpClient to ignore the self-signed certificate of the mock service
            // ONLY DO THIS IN DEVELOPMENT
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        // 2. Add MassTransit and configure it to use RabbitMQ
        services.AddMassTransit(x =>
        {
            // Add our consumer to the container
            x.AddConsumer<OrderReceivedEventConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                var host = hostContext.Configuration["RabbitMQ:Host"];
                cfg.Host(host, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // This tells MassTransit to set up a queue for this service and bind
                // it to the correct exchanges to receive the messages it's interested in.
                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();