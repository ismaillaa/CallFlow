using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence;

public class CallFlowDbContext : DbContext
{
    public CallFlowDbContext(DbContextOptions<CallFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Campagne> Campagnes { get; set;}
}

