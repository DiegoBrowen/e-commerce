namespace Home.Api.Providers;

public static class SwaggerProvider
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    public static void UseSwaggerProvider(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}