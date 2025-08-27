using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// An ASP.NET Core Web API that accepts JSON and returns a canned JSON route.
namespace MockRos.API.Controllers
{

    //c# records
    public record RouteRequest(string OrderId, Address Pickup, Address Dropoff);
    public record Address(string Street, string City, string PostalCode);
    public record RouteResponse(string OrderId, string RouteId, DateTime EstimatedDeliveryTime);
    
    
    [Route("[controller]")]
    [ApiController]
    public class RosController : ControllerBase
    {
        private readonly ILogger<RosController> _logger;

        public RosController(ILogger<RosController> logger)
        {
            _logger = logger;
        }

        [HttpPost("routes")]
        public async Task<IActionResult> CalculateRoute([FromBody] RouteRequest request)
        {
            _logger.LogInformation("--- Mock ROS Received a Route Request ---");
            _logger.LogInformation("Received for Order ID: {orderId}", request.OrderId);
            _logger.LogInformation("Pickup: {pickup}", request.Pickup);
            _logger.LogInformation("Dropoff: {dropoff}", request.Dropoff);
            
            
            //simulate the delay of a network call to a third party service
            await Task.Delay(TimeSpan.FromMilliseconds(300));

            var response = new RouteResponse(
                request.OrderId,
                $"ROUTE-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                DateTime.UtcNow.AddHours(4));
            
            _logger.LogInformation("Route calculated. Route ID:{routeId}\n",
                response.RouteId);

            return Ok(response);
        }
        
       
      
            
            
    }
}









