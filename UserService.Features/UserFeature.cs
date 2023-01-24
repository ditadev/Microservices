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
    private readonly UserDataContext _userDataContext;

    public UserFeature(UserDataContext userDataContext)
    {
        _userDataContext = userDataContext;
    }

    public async Task<User> AddUser(User user)
    {
        await using var transaction = await _userDataContext.Database.BeginTransactionAsync();
        _userDataContext.User.Add(user);
        await _userDataContext.SaveChangesAsync();

        var integrationEventData = JsonConvert.SerializeObject(new
        {
            id = user.ID,
            name = user.Name
        });

        _userDataContext.IntegrationEventOutbox.Add(
            new IntegrationEvent
            {
                Event = "user.add",
                Data = integrationEventData
            });

        await _userDataContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return user;
    }

    public async Task<List<User>> GetUsers()
    {
        return await _userDataContext.User.ToListAsync();
    }
}