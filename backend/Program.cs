using backend.Endpoints;
using StackExchange.Redis;
using backend.DataAccess.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.UsePostApi();

builder.Services.AddScoped<IRedisRepository, RedisRepository>();

var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrEmpty(redisConnection))
{
  throw new Exception("Redis connection string is null or empty");
}

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPostEndpoint();

Console.WriteLine("http://localhost:8080/swagger/index.html");

app.Run();
