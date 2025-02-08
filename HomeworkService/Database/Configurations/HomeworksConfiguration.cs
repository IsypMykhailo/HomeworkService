using HomeworkService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeworkService.Database.Configurations;

public class HomeworksConfiguration : IEntityTypeConfiguration<Homeworks>
{
    public void Configure(EntityTypeBuilder<Homeworks> builder)
    {
        builder
            .HasMany(e => e.Items)
            .WithOne(c => c.Homework);
    }
}