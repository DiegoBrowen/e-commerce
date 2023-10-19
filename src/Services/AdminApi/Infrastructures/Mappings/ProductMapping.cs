using Admin.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Admin.Api.Infrastructures.Mappings;

public class ProductMapping
    : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.Property(ci => ci.Id)
            .UseHiLo("products_hilo")
            .IsRequired();

        builder.Property(ci => ci.Name)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .IsRequired(true);

        builder.HasOne(ci => ci.Category)
            .WithMany()
            .HasForeignKey(ci => ci.CategoryId);
    }
}