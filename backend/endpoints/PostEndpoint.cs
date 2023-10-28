using backend.Service;

namespace backend.Endpoints;

public static class PostEndpoint
{
  public static IServiceCollection UsePostApi(this IServiceCollection service)
  {
    service.AddScoped<IPostService, PostService>();
    return service;
  }

  public static WebApplication MapPostEndpoint(this WebApplication app)
  {
    app.MapGet("/api/posts", (IPostService postService) => postService.GetNewestPostAsync());

    app.MapGet("/api/posts/{id}", (IPostService postService, int postId) => postService.GetPostById(postId));

    app.MapPost("/api/posts", (IPostService postService, string content) => postService.CreatePostAsync(content));

    app.MapPut("/api/posts/{id}", (IPostService postService, int id, string content) => postService.UpdatePostAsync(id, content));

    app.MapDelete("/api/posts/{id}", (IPostService postService, int id) => postService.DeletePostAsync(id));

    return app;
  }
}
