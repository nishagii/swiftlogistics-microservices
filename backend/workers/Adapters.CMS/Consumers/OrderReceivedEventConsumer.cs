using System.Text;
using Common.Contracts;
using MassTransit;

namespace Adapters.CMS.Consumers;

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
        _logger.LogInformation("Recieved OrderReceivedEvent: {OrderId}",message.OrderId);
        
        
        // --- This is where the integration logic lives ---
        
        // 1. Create an HttpClient to call the mock service
        var httpClient = _httpClientFactory.CreateClient("CmsClient");
        
        // 2. Manually construct the XML payload
        var xmlPayload = 
            $@"<order>
                <orderId>{message.OrderId}</orderId>
                <clientId>{message.ClientName.Replace(" ", "")}</clientId>
                <deliveryAddress>{message.DeliveryAddress}</deliveryAddress>
            </order>";
        
        // 3. Create the HTTP content with the correct Content-Type
        var content = new StringContent(xmlPayload, Encoding.UTF8, "application/xml");

        try
        {
            //make the POST request
            var response = await httpClient.PostAsync("/cms/order", content);

            //check the response and log the outcome
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully posted order {OrderId} to Mock CMS. Response: {Response}",
                    message.OrderId, responseBody);
            }
            else
            {
                _logger.LogError("Failed to post order {OrderId} to Mock CMS.Status Code: {StatusCode}",
                    message.OrderId, response.StatusCode);
                //In a real system, you might throw an exception here to trigger a retry or move to a dead-letter queue.
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred while calling Mock CMS for order {OrderId}", message.OrderId);
            throw; // Re-throwing the exception is important to let MassTransit know the message failed processing.
        }
    }
}