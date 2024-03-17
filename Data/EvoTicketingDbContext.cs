using EvoTicketing.Models;
using Microsoft.EntityFrameworkCore;

namespace EvoTicketing.Data;

public class EvoTicketingDbContext : DbContext
{
    public DbSet<Ticket> tickets {get; set; }
    public EvoTicketingDbContext(DbContextOptions<EvoTicketingDbContext> options ) : base(options)
    {
        
    }

}