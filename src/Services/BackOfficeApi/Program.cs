using Admin.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEntityFramework();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.RegisterDependencies();

var app = builder.Build();
app.UseSwaggerProvider();
app.UseAuthorization();
app.MapControllers();
app.Run();