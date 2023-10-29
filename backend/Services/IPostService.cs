namespace backend.Service;

public interface IPostService
{
  Task<IResult> GetAllPostsAsync();

  Task<IResult> GetNewestPostAsync();

  Task<IResult> GetPostById(int postId);

  Task<IResult> CreatePostAsync(string content);

  Task<IResult> UpdatePostAsync(int id, string content);

  Task<IResult> DeletePostAsync(int id);
}
