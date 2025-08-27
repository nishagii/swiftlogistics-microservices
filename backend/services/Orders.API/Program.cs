using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add MassTransit and configure it to use RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // Get the RabbitMQ host from appsettings.json
        var host = builder.Configuration["RabbitMQ:Host"];
        
        cfg.Host(host, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // This tells MassTransit to automatically configure the topology (exchanges, etc.)
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- Application Pipeline Configuration ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();