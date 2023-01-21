using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using UserService.Model.DataContext;

namespace UserService
{
    public class HostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public HostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var factory = new ConnectionFactory();
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    using var scope = _scopeFactory.CreateScope();
                     using var dbContext = scope.ServiceProvider.GetRequiredService<UserDataContext>();
                    var integrationEvents = dbContext.IntegrationEventOutbox.AsNoTracking().OrderBy(o => o.ID).ToList();
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
                    await Task.Delay(2000, stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    await Task.Delay(4000, stoppingToken);
                }
            }
        }
    }
}
