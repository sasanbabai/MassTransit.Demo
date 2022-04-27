using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Api;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IRequestClient<CheckOrder> _checkOrderRequestClient;
    private readonly IRequestClient<SubmitOrder> _submitOrderRequestClient;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public OrderController(ILogger<OrderController> logger,
        IRequestClient<CheckOrder> checkOrderRequestClient,
        IRequestClient<SubmitOrder> submitOrderRequestClient,
        ISendEndpointProvider sendEndpointProvider)
    {
        _logger = logger;
        _checkOrderRequestClient = checkOrderRequestClient;
        _submitOrderRequestClient = submitOrderRequestClient;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> Get(Guid orderId)
    {
        var (status, notFound) = await _checkOrderRequestClient.GetResponse<OrderStatus, OrderNotFound>(new
        {
            OrderId = orderId,
        });

        if (notFound.IsCompletedSuccessfully)
        {
            return NotFound((await notFound).Message);
        }

        return Ok((await status).Message);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder([FromForm] Guid orderId, [FromForm] Guid customerId)
    {
        var (accepted, rejected) = await _submitOrderRequestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new
        {
            OrderId = orderId,
            CustomerId = customerId,
            Timestamp = InVar.Timestamp,
        });

        if (accepted.IsCompletedSuccessfully)
        {
            return Accepted((await accepted).Message);
        }

        return BadRequest((await rejected).Message);
    }

    [HttpPost("submit-and-forget")]
    public async Task<IActionResult> SubmitOrderAndForget([FromForm] Guid orderId, [FromForm] Guid customerId)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"exchange:submit-order"));
        await sendEndpoint.Send<SubmitOrder>(new
        {
            OrderId = orderId,
            CustomerId = customerId,
            Timestamp = InVar.Timestamp,
        });

        return Accepted();
    }
}