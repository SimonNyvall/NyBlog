using System.Text.Json;
using backend.Models;
using StackExchange.Redis;

namespace backend.DataAccess.Redis;

public class RedisRepository : IRedisRepository
{
  private readonly IDatabase _redis;
  private readonly ILogger<RedisRepository> _logger;

  public RedisRepository(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> logger)
  {
    _redis = connectionMultiplexer.GetDatabase();
    _logger = logger;
  }

  public async Task<Post[]> GetAllPostsAsync()
  {
    var postsJson = await _redis.ListRangeAsync("posts");
    
    if (postsJson is null)
    {
      _logger.LogWarning("No posts found in Redis");
      return Array.Empty<Post>();
    }
    
    var posts = postsJson.Select(p => JsonSerializer.Deserialize<Post>(p!)).ToArray();

    _logger.LogInformation("Posts found in Redis");
    
    return posts!;
  }

  public async Task AddPostAsync(Post post)
  {
    var postsJson = await _redis.ListRangeAsync("posts");

    var posts = postsJson.Select(p => JsonSerializer.Deserialize<Post>(p!)).ToList();
    
    if (posts is null || posts.Count == 0)
    {
      await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(post));
      return;
    }

    posts.Add(post);
    posts = ReIndexPosts(posts);

    // Delete all posts
    await _redis.KeyDeleteAsync("posts");

    // Add all posts
    foreach (var p in posts)
    {
      await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(p));
    }
  }

  private List<Post> ReIndexPosts(List<Post> posts)
  {
    for (var i = 0; i < posts.Count; i++)
    {
      posts[i].Id = posts.Count - i;
    }

    return posts;
  }

  public async Task UpdatePostAsync(Post post)
  {
    throw new NotImplementedException();
  }

  public async Task DeletePostAsync(string id)
  {
    throw new NotImplementedException();
  }
}

