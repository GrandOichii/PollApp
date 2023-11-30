
namespace PollApp.Api.Data;

public class DataContext : DbContext {
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }

    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // modelBuilder.Entity<PollOption>().Property(poll => poll.ID).ValueGeneratedOnAdd();
        // modelBuilder.Entity<Poll>().Property(poll => poll.ID).ValueGeneratedOnAdd();
    }
}