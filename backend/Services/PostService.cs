using backend.DataAccess.Redis;
using backend.Models;
using Markdig;

namespace backend.Service;

public class PostService : IPostService
{
  private readonly IRedisRepository _repository;
  private readonly ILogger<PostService> _logger;

  public PostService(IRedisRepository repository, ILogger<PostService> logger)
  {
    _repository = repository;
    _logger = logger;
  }

  public async Task<IResult> GetAllPostsAsync()
  {
    _logger.LogInformation("Getting all posts");

    var posts = await _repository.GetAllPostsAsync();

    _logger.LogInformation("Found {Count} posts", posts.Length);

    if (posts.Length == 0) return TypedResults.NoContent();

    return TypedResults.Ok(posts);
  }

  public async Task<IResult> GetNewestPostAsync()
  {
    _logger.LogInformation("Getting newest post");

    var posts = await _repository.GetAllPostsAsync();

    _logger.LogInformation("Found {Count} posts", posts.Length);

    if (posts.Length == 0) return TypedResults.NoContent();

    var newestPost = posts.OrderByDescending(p => p.CreatedAt).First();
    
    if (newestPost == null) return TypedResults.NoContent();

    _logger.LogInformation("Found newest post {Post}", newestPost);

    var html = Markdown.ToHtml(newestPost.Content);

    _logger.LogInformation("Converted markdown to html {Html}", html);
 
    return TypedResults.Ok(html);
  }

  public async Task<IResult> GetPostById(int postId)
  {
    _logger.LogInformation("Getting post by id {Id}", postId);

    var posts = await _repository.GetAllPostsAsync();

    if (posts.Length == 0) return TypedResults.NoContent();

    var nextPost = posts.Where(p => p.Id == postId).Last();

    if (nextPost == null) {
      _logger.LogInformation("No post found with id {Id}", postId);
      return TypedResults.NoContent();
    } 
    var html = Markdown.ToHtml(nextPost.Content);

    _logger.LogInformation("Converted markdown to html {Html}", html);
    return TypedResults.Ok(html);
  }

  public async Task<IResult> CreatePostAsync(string content)
  {
    var posts = await _repository.GetAllPostsAsync();

    var numberOfPosts = posts.Length;

    var post = new Post
    {
      Id = numberOfPosts + 1,
      CreatedAt = DateTime.Now,
      Content = content
    };

    await _repository.AddPostAsync(post);

    return TypedResults.Created("api/posts", post);
  }

  public async Task<IResult> UpdatePostAsync(int id, string content)
  {
    var post = new Post
    {
      Id = id,
      CreatedAt = DateTime.Now,
      Content = content
    };

    await _repository.UpdatePostAsync(post);

    _logger.LogInformation("Updated post with id {Id}", id);

    return TypedResults.Ok(post);
  }

  public async Task<IResult> DeletePostAsync(int id)
  {
    await _repository.DeletePostAsync(id);

    _logger.LogInformation("Deleted post with id {Id}", id);

    return TypedResults.Ok();
  }
}
