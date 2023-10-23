using Admin.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Admin.Api.Infrastructures;

public class AdminContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    public AdminContext(DbContextOptions<AdminContext> options)
        : base(options)
    {
    }
}