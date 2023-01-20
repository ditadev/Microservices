using Microsoft.AspNetCore.Mvc;
using PostService.Domain.Model;
using PostService.Features;

namespace PostService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostFeature _postFeature;

    public PostController(PostFeature postFeature)
    {
        _postFeature = postFeature;
    }

    [HttpPost]
    public async Task<IActionResult> AddPost(Post post)
    {
        return Ok(await _postFeature.AddPost(post));
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        return Ok(await _postFeature.GetPosts());
    }
}