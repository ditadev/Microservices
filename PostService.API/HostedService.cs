using System.Text;
using Newtonsoft.Json.Linq;
using PostService.Domain.DataContext;
using PostService.Domain.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PostService;

public class HostedService : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private readonly object _lock = new();
    private readonly ILogger<HostedService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IModel _channel;
    private IConnection _connection;

    public HostedService(ILogger<HostedService> logger, ConnectionFactory factory, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _factory = factory;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            lock (_lock)
            {
                var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<PostDataContext>();
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(" [x] Received {0}", message);
                var data = JObject.Parse(message);
                var type = ea.RoutingKey;

                switch (type)
                {
                    case "user.add":
                        if (dbContext.User.Any(a => a.ID == data["id"].Value<int>()))
                        {
                            _logger.LogInformation("Ignoring old/duplicate entity");
                        }
                        else
                        {
                            dbContext.User.Add(new User
                            {
                                ID = data["id"].Value<int>(),
                                Name = data["name"].Value<string>()
                            });
                            dbContext.SaveChangesAsync(stoppingToken);
                        }

                        break;
                    // case "user.update":
                    //     int newVersion = data["version"].Value<int>();
                    //     var user = dbContext.User.First(a => a.ID == data["id"].Value<int>());
                    //     if (user.Version >= newVersion)
                    //     {
                    //         Console.WriteLine("Ignoring old/duplicate entity");
                    //     }
                    //     else
                    //     {
                    //         user.Name = data["newname"].Value<string>();
                    //         user.Version = newVersion;
                    //         await dbContext.SaveChangesAsync(stoppingToken);
                    //     }
                    //     break;
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
        };
        _channel.BasicConsume("user.postservice", true, consumer);
        _channel.ConfirmSelect();
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hosted Service ended");
        _channel?.Dispose();
        _connection?.Dispose();
        return base.StopAsync(cancellationToken);
    }

    public sealed override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}