using Microsoft.EntityFrameworkCore;
using PostService.Domain.DataContext;
using PostService.Domain.Model;

namespace PostService.Features;

public class PostFeature
{
    private readonly PostDataContext _postDataContext;

    public PostFeature(PostDataContext postDataContext)
    {
        _postDataContext = postDataContext;
    }

    public async Task<Post> AddPost(Post post)
    {
        var newPost = new Post
        {
            PostId = post.PostId,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            User = await _postDataContext.User.FirstOrDefaultAsync(x => x.ID == post.UserId)
        };
        _postDataContext.Post.Add(newPost);
        await _postDataContext.SaveChangesAsync();
        return post;
    }

    public async Task<List<Post>> GetPosts()
    {
        return await _postDataContext.Post.Include(x => x.User).ToListAsync();
    }
}