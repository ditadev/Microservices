using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using UserService.Model.DataContext;
using UserService.Model.Model;

namespace UserService.Features;

public class UserFeature
{
    private readonly ILogger<UserFeature> _logger;
    private readonly UserDataContext _userDataContext;

    public UserFeature(UserDataContext userDataContext, ILogger<UserFeature> logger)
    {
        _userDataContext = userDataContext;
        _logger = logger;
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
        PublishToMessageQueue("user.add", integrationEventData);
        _logger.LogInformation($"user message queued from add user method{integrationEventData}");
        return user;
    }

    public async Task<List<User>> GetUsers()
    {
        return await _userDataContext.User.ToListAsync();
    }

    private void PublishToMessageQueue(string integrationEvent, string eventData)
    {
        // TOOO: Reuse and close connections and channel, etc, 
        var factory = new ConnectionFactory();
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();
        var body = Encoding.UTF8.GetBytes(eventData);
        channel.BasicPublish("user",
            integrationEvent,
            null,
            body);
        _logger.LogInformation($"user message published{body.Length}");
    }
}