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
    await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(post));

    _logger.LogInformation("Post added to Redis");
  }
}

