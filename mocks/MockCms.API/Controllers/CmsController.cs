using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MockCms.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CmsController : ControllerBase
    {
        private readonly ILogger<CmsController> _logger;

        public CmsController(ILogger<CmsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("orders")]
        //tells ASP.NET endpoint accepts XML
        [Consumes("application/xml", "text/xml")]
        public async Task<IActionResult> SubmitOrder()
        {
            //read the raw XML string from the request body
            using var reader = new StreamReader(Request.Body);
            var xmlContent = await reader.ReadToEndAsync();
            
            _logger.LogInformation("--- Mock CMS Received an Order ---");
            _logger.LogInformation("Payload:\n{xml}", xmlContent);
            
            //legacy system vibes baby
            // important for testing the asynchronous nature of my middleware
            await Task.Delay(TimeSpan.FromMicroseconds(1000));
            
            // SOAP - XML based messaging format
            // In real systems they return a SOAP envelope on success

            var responseId = Guid.NewGuid();
            _logger.LogInformation("Order processed successfully. Response ID: {id}\\n",responseId);
            return Ok(new { status = "Order Received" , cmsConfirmationId = responseId});

        }
    }
}
