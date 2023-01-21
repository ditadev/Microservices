using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using UserService.Model.DataContext;
using UserService.Model.Model;

namespace UserService.Features;

public class UserFeature : IDisposable
{
    private readonly ILogger<UserFeature> _logger;
    private readonly UserDataContext _userDataContext;
    private IModel _channel;
    private IConnection _connection;

    public UserFeature(UserDataContext userDataContext, ILogger<UserFeature> logger, ConnectionFactory factory)
    {
        _userDataContext = userDataContext;
        _logger = logger;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }
    
    public async Task<User> AddUser(User user)
    {
        _userDataContext.User.Add(user);
        await _userDataContext.SaveChangesAsync();
        var integrationEventData = JsonConvert.SerializeObject(new
        {
            id = user.ID,
            name = user.Name
        });
        var body = Encoding.UTF8.GetBytes(integrationEventData);
        _channel.BasicPublish("user",
            "user.add",
            null,
            body);
        _logger.LogInformation($"user message published {body.Length}");
        return user;
    }

    public async Task<List<User>> GetUsers()
    {
        return await _userDataContext.User.ToListAsync();
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    ~UserFeature()
    {
        Dispose();
    }
}
