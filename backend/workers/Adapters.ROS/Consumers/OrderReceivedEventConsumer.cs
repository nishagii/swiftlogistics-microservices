// /backend/workers/Adapters.ROS/Consumers/OrderReceivedEventConsumer.cs
using System.Net.Http.Json;
using Common.Contracts;
using MassTransit;

namespace Adapters.ROS.Consumers;

public class OrderReceivedEventConsumer : IConsumer<OrderReceivedEvent>
{
    private readonly ILogger<OrderReceivedEventConsumer> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderReceivedEventConsumer(ILogger<OrderReceivedEventConsumer> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task Consume(ConsumeContext<OrderReceivedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received OrderReceivedEvent: {OrderId}", message.OrderId);

        var httpClient = _httpClientFactory.CreateClient("RosClient");

        // Use an anonymous object to create the structure the Mock ROS API expects.
        // This will be automatically serialized into JSON.
        var requestBody = new
        {
            orderId = message.OrderId.ToString(),
            pickup = new { street = "123 Warehouse St", city = "Anytown", postalCode = "12345" },
            dropoff = new { street = message.DeliveryAddress, city = "Otherville", postalCode = "67890" }
        };

        try
        {
            // Use the modern JsonContent helper to create the request body.
            var response = await httpClient.PostAsJsonAsync("/ros/routes", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<object>();
                _logger.LogInformation("Successfully posted order {OrderId} to Mock ROS. Response: {Response}", message.OrderId, responseData);
            }
            else
            {
                _logger.LogError("Failed to post order {OrderId} to Mock ROS. Status Code: {StatusCode}", message.OrderId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while calling Mock ROS for order {OrderId}", message.OrderId);
            throw;
        }
    }
}