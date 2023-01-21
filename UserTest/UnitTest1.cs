using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using UserService.Features;
using UserService.Model.DataContext;
using UserService.Model.Model;
using Xunit;

namespace UserTest;

public class UserFeatureTests
{
    private readonly Mock<UserDataContext> _mockUserDataContext = new Mock<UserDataContext>();
    private readonly Mock<ILogger<UserFeature>> _mockLogger = new Mock<ILogger<UserFeature>>();
    private readonly UserFeature _userFeature;
    private readonly Mock<ConnectionFactory> _mockFactory = new Mock<ConnectionFactory>();


    public UserFeatureTests()
    {
        _mockUserDataContext.Setup(x => x.User.Add(It.IsAny<User>()));
        _mockUserDataContext.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockUserDataContext.Setup(x => x.User.ToListAsync()).ReturnsAsync(new List<User>());

        _mockFactory.Setup(x => x.CreateConnection()).Returns(new Mock<IConnection>().Object);

        _userFeature = new UserFeature(_mockUserDataContext.Object, _mockLogger.Object, _mockFactory.Object);
    }

    [Fact]
    public async Task AddUser_WhenCalled_AddsUserToDataContext()
    {
        var user = new User { ID = 1, Name = "John Doe" };

        await _userFeature.AddUser(user);

        _mockUserDataContext.Verify(x => x.User.Add(user), Times.Once);
        _mockUserDataContext.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUsers_WhenCalled_RetrievesUsersFromDataContext()
    {
        var users = await _userFeature.GetUsers();

        _mockUserDataContext.Verify(x => x.User.ToListAsync(), Times.Once);
        Assert.IsType<List<User>>(users);
    }

    [Fact]
    public void Dispose_WhenCalled_DisposeConnectionAndChannel()
    {
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IModel>();
        _mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);
        mockConnection.Setup(x => x.CreateModel()).Returns(mockChannel.Object);

        var userFeature = new UserFeature(_mockUserDataContext.Object, _mockLogger.Object, _mockFactory.Object);
        userFeature.Dispose();

        mockChannel.Verify(x => x.Dispose(), Times.Once);
        mockConnection.Verify(x => x.Dispose(), Times.Once);
        _mockFactory.Verify(x=>x.Dispose(), Times.Once);
    }
}