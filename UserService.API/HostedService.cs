using System.Text;
using RabbitMQ.Client;
using UserService.Model.DataContext;

namespace UserService;

 public class HostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public HostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            using var scope = _scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<UserDataContext>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishOutstandingIntegrationEvents(stoppingToken);
            }
        }

        private async Task PublishOutstandingIntegrationEvents(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory();
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                while (!stoppingToken.IsCancellationRequested)
                {
                    {
                        using var scope = _scopeFactory.CreateScope();
                        using var dbContext = scope.ServiceProvider.GetRequiredService<UserDataContext>();
                        var integrationEvents = dbContext.IntegrationEventOutbox.OrderBy(o => o.ID).ToList();
                        foreach (var integrationEvent in integrationEvents)
                        {
                            var body = Encoding.UTF8.GetBytes(integrationEvent.Data);
                            channel.BasicPublish(exchange: "user",
                                                             routingKey: integrationEvent.Event,
                                                             basicProperties: null,
                                                             body: body);
                            Console.WriteLine("Published: " + integrationEvent.Event + " " + integrationEvent.Data);
                            dbContext.Remove(integrationEvent);
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                    await Task.Delay(1000, stoppingToken);
                }     
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                await Task.Delay(5000, stoppingToken);
            }
        }
    }