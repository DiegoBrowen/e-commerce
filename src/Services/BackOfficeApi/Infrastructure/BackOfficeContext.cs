using BackOffice.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Api.Infrastructure;

public class BackOfficeContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    public BackOfficeContext(DbContextOptions<BackOfficeContext> options)
        : base(options)
    {
    }
}