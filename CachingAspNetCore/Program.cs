using CachingAspNetCore.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
builder.Services.AddControllers();
// output caching
builder.Services.AddOutputCache(options =>
{
  options.AddBasePolicy(builder =>
  {
    builder.Expire(TimeSpan.FromMinutes(2));
  });
  options.AddPolicy("ExpiresIn15s", builder => builder.Expire(TimeSpan.FromSeconds(15)));
  options.AddPolicy("evict",
  builder =>
  {
    builder.Expire(TimeSpan.FromSeconds(90))
           .Tag("tag-book");
  });
  options.AddPolicy("NoLock", builder => builder.SetLocking(false));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IBookRepository, BookRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseOutputCache();
app.MapControllers();
// output cache
app.Run();
