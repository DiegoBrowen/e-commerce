using Home.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwagger();
builder.RegisterDependencies();

var app = builder.Build();
app.UseSwaggerProvider();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSubscriptions();
app.MapControllers();

app.Run();