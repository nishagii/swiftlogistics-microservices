using System.ComponentModel.DataAnnotations;

namespace Orders.API;

public class CreateOrderDto
{
    [Required] public string ClientName { get; set; }
    
    [Required]
    [MinLength(10)]
    public string DeliveryAddress { get; set; }
    
    
}