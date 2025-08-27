using Common.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Orders.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrdersController> _logger;

        // MassTransit and the Logger are injected
        public OrdersController(ILogger<OrdersController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            _logger.LogInformation("Received new order from client: {ClientName}",orderDto.ClientName);

            // Create the event message object
            var orderReceivedEvent = new OrderReceivedEvent
            {
                OrderId = Guid.NewGuid(),
                ClientName = orderDto.ClientName,
                DeliveryAddress = orderDto.DeliveryAddress,
                Timestamp = DateTime.UtcNow
            };
        }
    }
    
     
}
