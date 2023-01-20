using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PostService.Domain.DataContext;
using PostService.Domain.Model;

namespace PostService.Features;

public class PostFeature
{
    private readonly ILogger<PostFeature> _logger;
    private readonly PostDataContext _postDataContext;

    public PostFeature(PostDataContext postDataContext, ILogger<PostFeature> logger)
    {
        _postDataContext = postDataContext;
        _logger = logger;
    }

    public async Task<Post> AddPost(Post post)
    {
        var posti = new Post
        {
            PostId = post.PostId,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            User = await _postDataContext.User.FirstOrDefaultAsync(x => x.ID == post.UserId)
        };
        _postDataContext.Post.Add(posti);
        await _postDataContext.SaveChangesAsync();
        return post;
    }

    public async Task<List<Post>> GetPosts()
    {
        return await _postDataContext.Post.Include(x => x.User).ToListAsync();
    }
}