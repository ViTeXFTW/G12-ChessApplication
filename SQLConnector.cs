using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using G12_ChessApplication;

public class SQLConnector
{
    private AppDbContext dbContext;

    public SQLConnector()
    {
        dbContext = new AppDbContext();
    }

    public void InitializeDatabase()
    {
        dbContext.Database.EnsureCreated();
    }

    //create match constructor:
    
    
    
    public async Task<User> AuthenticateUser(string username, string password)
    {
        using (var context = new AppDbContext())
        {
            // Query the database asynchronously to find the user by username and password
            return await context.Login.FirstOrDefaultAsync(user => user.Username == username && user.Passw == password);
        }
    }

    public string CreateUser(string username, string password)
    {
        using (var context = new AppDbContext())
        {
            // Check if the user already exists in the database
            var existingUser = context.Login.FirstOrDefault(user => user.Username == username);
            if (existingUser != null)
            {
                return "Username already exists. Please choose a different username.";
            }

            // Create a new user
            var newUser = new User
            {
                Username = username,
                Passw = password
            };

            // Add the new user to the database
            context.Login.Add(newUser);
            context.SaveChanges();

            return "Account created successfully! You can now log in.";
        }
    }
    

    public class User
    {
        [Key]
        public string Username { get; set; }
        public string Passw { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
    
    public class Match
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; }
        public string Opponent { get; set; }
        public int Result { get; set; }
        public User UserNavigation { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Login { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Server=mysql-makki12.alwaysdata.net;Database=makki12_chess;User=makki12;Password=ChessGame_;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Username);
            modelBuilder.Entity<Match>()
                .HasKey(m => m.Id);

            // Configure the foreign key relationship
            modelBuilder.Entity<Match>()
                .HasOne(m => m.UserNavigation)
                .WithMany(u => u.Matches)
                .HasForeignKey(m => m.User)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed the database with an admin and a test user
            modelBuilder.Entity<User>().HasData(
                new User { Username = "admin", Passw = "admin" },
                new User { Username = "test", Passw = "123" }
            );
        }
    }
    
    public async Task AddMatchResult(string user, string opponent, int result)
    {
        using (var context = new AppDbContext())
        {
            var match = new Match
            {
                User = user,
                Opponent = opponent,
                Result = result
            };

            context.Matches.Add(match);
            await context.SaveChangesAsync();
        }
    }
    
    public async Task<List<LeaderboardEntry>> GetLeaderboard()
    {
        using (var context = new AppDbContext())
        {
            // Aggregate match results to calculate wins, losses, and draws for each user
            var leaderboardData = await context.Matches
                .GroupBy(m => m.User)
                .Select(g => new
                {
                    Username = g.Key,
                    Wins = g.Count(m => m.Result == 2),
                    Losses = g.Count(m => m.Result == 1),
                    Draws = g.Count(m => m.Result == 0),
                    TotalGames = g.Count()
                })
                .ToListAsync();

            // Convert the aggregated data to a list of LeaderboardEntry objects
            return leaderboardData.Select(entry => new LeaderboardEntry(
                entry.Username,
                entry.Wins,
                entry.Losses,
                entry.Draws
            )).ToList();
        }
    }
    
    
    
    
    
}
