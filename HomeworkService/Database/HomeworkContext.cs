using Microsoft.EntityFrameworkCore;
using HomeworkService.Database.Configurations;
using HomeworkService.Domain.Models;

namespace HomeworkService.Database;

public class HomeworkContext : DbContext
{
    public HomeworkContext(DbContextOptions<HomeworkContext> options) : base(options) { }

    public DbSet<Homeworks> Homeworks { get; set; } = null!;
    public DbSet<HomeworkItems> HomeworkItems { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HomeworksConfiguration());
    }
}