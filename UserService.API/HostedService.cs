using System.Text;
using RabbitMQ.Client;
using UserService.Model.DataContext;

namespace UserService;

public class HostedService : BackgroundService
{
    private readonly ILogger<HostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public HostedService(IServiceScopeFactory scopeFactory, ILogger<HostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var factory = new ConnectionFactory();
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ConfirmSelect(); 
                IBasicProperties props = channel.CreateBasicProperties();
                props.DeliveryMode = 2; 

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();
                    using var dbContext = scope.ServiceProvider.GetRequiredService<UserDataContext>();
                    var integrationEvents =
                        dbContext.IntegrationEventOutbox.OrderBy(o => o.ID).ToList();

                    foreach (var integrationEvent in integrationEvents)
                    {
                        var body = Encoding.UTF8.GetBytes(integrationEvent.Data);
                        channel.BasicPublish("user",
                            integrationEvent.Event,
                            null,
                            body);
                        channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5)); 
                        _logger.LogInformation("Published: " + integrationEvent.Event + " " +
                                               integrationEvent.Data);
                        dbContext.Remove(integrationEvent);
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }

                    await Task.Delay(2000, stoppingToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                await Task.Delay(4000, stoppingToken);
            }
    }
}