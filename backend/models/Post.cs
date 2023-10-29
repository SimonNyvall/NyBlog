namespace backend.Models;

public class Post
{
  public int Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public string Content { get; set; } = string.Empty;
}
