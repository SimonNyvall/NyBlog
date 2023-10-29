using backend.Models;

namespace backend.DataAccess.Redis;

public interface IRedisRepository
{
  Task<Post[]> GetAllPostsAsync();

  Task AddPostAsync(Post post);

  Task UpdatePostAsync(Post post);

  Task DeletePostAsync(int id);
}
