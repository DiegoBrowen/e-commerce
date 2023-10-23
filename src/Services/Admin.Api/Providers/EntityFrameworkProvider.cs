using Admin.Api.Infrastructures;
using Microsoft.EntityFrameworkCore;

namespace Admin.Api.Providers;

public static class EntityFrameworkProvider
{
    public static void AddEntityFramework(this IServiceCollection services)
    {
        services.AddDbContext<AdminContext>(
            options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
    }
}