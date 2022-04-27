using MassTransit;

namespace Worker;

public class MassTransitHostedService : BackgroundService
{
    private readonly ILogger<MassTransitHostedService> _logger;
    private readonly IBusControl _busControl;

    public MassTransitHostedService(ILogger<MassTransitHostedService> logger, IBusControl busControl)
    {
        _logger = logger;
        _busControl = busControl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _busControl.StartAsync(stoppingToken);
        }
    }
}
