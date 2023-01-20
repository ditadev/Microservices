using System.Text;
using Newtonsoft.Json.Linq;
using PostService.Domain.DataContext;
using PostService.Domain.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PostService;

public class HostedService : BackgroundService
{
    private readonly ILogger<HostedService> _logger;

    public HostedService(ILogger<HostedService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var dbContext = new PostDataContext();

            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation(" [x] Received {0}", message);

            var data = JObject.Parse(message);
            var type = ea.RoutingKey;
            if (type == "user.add")
            {
                dbContext.User.Add(new User
                {
                    ID = data["id"].Value<int>(),
                    Name = data["name"].Value<string>()
                });
                dbContext.SaveChangesAsync();
            }
            else if (type == "user.update")
            {
                var user = dbContext.User.First(a => a.ID == data["id"].Value<int>());
                user.Name = data["newname"].Value<string>();
                dbContext.SaveChangesAsync();
            }
        };
        channel.BasicConsume("user.postservice",
            true,
            consumer);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service ended");

        return base.StopAsync(cancellationToken);
    }

    public sealed override void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}