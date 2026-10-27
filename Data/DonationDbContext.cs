using DonationTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DonationTask.Data;

public class DonationDbContext : DbContext
{
    public DonationDbContext(
        DbContextOptions<DonationDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Donation> Donations { get; set; }
}