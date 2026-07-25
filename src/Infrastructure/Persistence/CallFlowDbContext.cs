using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence;

public class CallFlowDbContext : DbContext
{
    public CallFlowDbContext(DbContextOptions<CallFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Campagne> Campagnes { get; set;}
    public DbSet<Prospect> Prospects { get; set;}
    public DbSet<Agent> Agents { get; set;}
    public DbSet<Appel> Appels { get; set;}
    public DbSet<Rappel> Rappels { get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Prospect>()
            .HasIndex(p => new { p.CampagneId, p.Telephone })
            .IsUnique();
    }
    
}

