using Microsoft.AspNetCore.Mvc;
using UserService.Features;
using UserService.Model.Model;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserFeature _userFeature;

    public UserController(UserFeature userFeature)
    {
        _userFeature = userFeature;
    }

    [HttpPost]
    public async Task<ActionResult> AddUser(User user)
    {
        return Ok(await _userFeature.AddUser(user));
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        return Ok(await _userFeature.GetUsers());
    }
}