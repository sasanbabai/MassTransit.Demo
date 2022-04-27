using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ILogger<CustomerController> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public CustomerController(ILogger<CustomerController> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid customerId)
    {
        await _publishEndpoint.Publish<CustomerDeleted>(new
        {
            CustomerId = customerId,
        });

        return Ok();
    }
}