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
    var posts = await GetAllPosts();

    if (posts.Count == 0)
    {
        await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(post));
        return;
    }

    var updatedPosts = new List<Post>(posts!);
    updatedPosts.Add(post);

    var reIndexedPosts = ReIndexPosts(updatedPosts.ToArray());
    updatedPosts = reIndexedPosts?.ToList() ?? posts!;

    if (updatedPosts is null)
    {
      _logger.LogWarning("Posts not found in Redis");
      return;
    }

    // Delete all posts
    await _redis.KeyDeleteAsync("posts");

    // Add all posts
    foreach (var p in updatedPosts)
    {
      await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(p));
    }
  }

  public async Task UpdatePostAsync(Post post)
  {
    var posts = await GetAllPosts();

    if (posts.Count == 0)
    {
      _logger.LogWarning("Posts not found in Redis");
      return;
    }

    var postToUpdate = posts.FirstOrDefault(p => p!.Id == post.Id);

    if (postToUpdate is null)
    {
      _logger.LogWarning("Post not found in Redis");
      return;
    }
    
    posts.Remove(postToUpdate);
    
    posts.Insert(postToUpdate.Id - 1, post);

    // Delete all posts 
    await _redis.KeyDeleteAsync("posts");

    foreach (var p in posts)
    {
      await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(p));
    }
  }

  public async Task DeletePostAsync(int id)
  {
    var posts = await GetAllPosts();

    if (posts.Count == 0)
    {
      _logger.LogWarning("Posts not found in Redis");
      return;
    }

    var postToDelete = posts.FirstOrDefault(p => p!.Id == id);

    if (postToDelete is null)
    {
      _logger.LogWarning("Post not found in Redis");
      return;
    }
    
    var updatedPosts = new List<Post>(posts!);

    updatedPosts.Remove(postToDelete);

    var reIndexedPosts = ReIndexPosts(updatedPosts.ToArray()!);

    updatedPosts = ReIndexPosts(updatedPosts!).ToList() ?? posts!;

    await _redis.KeyDeleteAsync("posts");

    // Add all posts 
    foreach (var p in updatedPosts)
    {
      await _redis.ListRightPushAsync("posts", JsonSerializer.Serialize(p));
    }
  }

  private async Task<List<Post?>> GetAllPosts()
  {
    var postsJson = await _redis.ListRangeAsync("posts");
    
    var posts = postsJson
        .Select(p => {
            try
            {
                return JsonSerializer.Deserialize<Post>(p!);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize post");
                return null;
            }
        })
        .Where(p => p != null)
        .ToList();

    return posts!;
  }

  private IEnumerable<Post> ReIndexPosts(IEnumerable<Post> posts)
  {    
    var postsList = new List<Post>(posts);

    for (var i = 0; i < postsList.Count; i++)
    {
      postsList[i].Id = postsList.Count - i;
    }

    return postsList;
  }
}

